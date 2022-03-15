﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class PaymentService : EntityServiceBase<Payment>
    {
        protected override IQueryable<Payment> DefaultQuery => _context.Payment
                                                                .Include(x => x.PaymentItem)
                                                                    .ThenInclude(x => x.Person)
                                                                        .ThenInclude(x => x.PaidFrom)
                                                                .Include(x => x.PaymentItem)
                                                                    .ThenInclude(x => x.Timesheet)
                                                                        .ThenInclude(x => x.Person);
        private readonly PaymentOptions _paymentOptions;
        private readonly TimesheetService _timesheetService;
        public PaymentService(IOptions<PaymentOptions> paymentOptions, TimesheetContext context, TimesheetService timesheetService) : base(context)
        {
            _paymentOptions = paymentOptions.Value;
            _timesheetService = timesheetService;
        }

        public override async Task<Payment> GetAsync(int id)
        {
            return await DefaultQuery
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<List<Payment>> GetAsync(bool asNoTracking = true)
        {
            var payments = DefaultQuery;
            return await (asNoTracking ? payments.AsNoTracking().ToListAsync() : payments.ToListAsync());
        }

        public override async Task SaveAsync(Payment entity)
        {
            if (entity.Id > 0)
            {
                var tracked = _context.ChangeTracker.Entries();
                SetModified(entity);
            }
            else
            {
                entity.CreateTime = DateTime.Now;
                foreach (var item in entity.PaymentItem)
                {
                    item.CreateTime = DateTime.Now;
                }
                _context.Set<Payment>().Add(entity);
            }
            await _context.SaveChangesAsync();
        }

        public async Task GenerateItemsAsync(Payment payment, IEnumerable<int> selectedTimesheetIds)
        {
            if (payment.Id > 0)
            {
                var oldTimesheets = await _timesheetService.GetPaymentTimesheetIdsAsync(payment.Id);
                var timesheetsToRemove = new List<int>();
                var timesheetsToAdd = new List<int>();
                foreach (var timesheetId in oldTimesheets)
                {
                    if (!selectedTimesheetIds.Any(x => x == timesheetId))
                    {
                        timesheetsToRemove.Add(timesheetId);
                    }
                }

                foreach (var timesheetId in selectedTimesheetIds)
                {
                    if (!oldTimesheets.Any(x => x == timesheetId))
                        timesheetsToAdd.Add(timesheetId);
                }

                //Remove
                foreach (var timesheetId in timesheetsToRemove)
                {
                    var timesheet = await _context.Timesheet.AsNoTracking().Where(x => x.Id == timesheetId).FirstOrDefaultAsync();
                    var paymentItemId = timesheet.PaymentItemId;
                    timesheet.PaymentItemId = null;
                    await _timesheetService.SaveAsync(timesheet);

                    var paymentItemQuery = _context.PaymentItem.Include(x => x.Timesheet).Where(x => x.Id == paymentItemId);
                    var anyTimesheetsInPaymentItem = await paymentItemQuery.Select(x => x.Timesheet.Any()).FirstAsync();

                    if (!anyTimesheetsInPaymentItem)
                    {
                        _context.PaymentItem.Remove(await paymentItemQuery.FirstOrDefaultAsync());
                        await _context.SaveChangesAsync();
                    }
                }
                //Add
                foreach (var timesheetId in timesheetsToAdd)
                {
                    var timesheet = await _context.Timesheet.AsNoTracking().Where(x => x.Id == timesheetId).FirstOrDefaultAsync();
                    _context.Entry(timesheet).State = EntityState.Detached;
                    var paymentItemQuery = _context.PaymentItem.Include(x => x.Timesheet).AsNoTracking().Where(x => x.PaymentId == payment.Id && x.PersonId == timesheet.PersonId && x.Year == timesheet.Year && x.Month == timesheet.Month);
                    //create new one
                    if (!paymentItemQuery.Any())
                    {
                        var item = new PaymentItem()
                        {
                            PaymentId = payment.Id,
                            PersonId = timesheet.PersonId,
                            Year = timesheet.Year,
                            Month = timesheet.Month,
                            CreateTime = DateTime.Now
                        };
                        _context.PaymentItem.Add(item);
                        await _context.SaveChangesAsync();
                    }
                    var paymentItem = paymentItemQuery.First();
                    paymentItem.Timesheet.Add(timesheet);
                    timesheet.PaymentItemId = paymentItem.Id;
                    await _timesheetService.SaveAsync(timesheet);
                }
            }
            else
            {
                var timesheets = await _timesheetService.GetMultipleAsync(selectedTimesheetIds);

                var group = timesheets
                    .GroupBy(x => new { x.PersonId, x.Month, x.Year })
                    ;
                payment.PaymentItem = group
                    .Select(x => new PaymentItem()
                    {
                        PaymentId = payment.Id,
                        PersonId = x.Key.PersonId,
                        Month = x.Key.Month,
                        Year = x.Key.Year,
                        Timesheet = x.ToList(),
                        Reward = x.Select(x => x.Reward ?? 0).Sum()
                    })
                    .ToList();
                foreach (var item in payment.PaymentItem)
                {
                    CalculateRewards(item);
                }
            }
        }

        public override async Task RemoveAsync(Payment entity)
        {
            _context.Set<PaymentItem>().RemoveRange(entity.PaymentItem);
            await _context.SaveChangesAsync();
            await base.RemoveAsync(entity);
        }

        private void CalculateRewards(PaymentItem paymentItem)
        {
            var hasTax = _context.Person.Where(x => x.Id == paymentItem.Id).Select(x => x.HasTax).FirstOrDefault();
            paymentItem.Tax = hasTax ? Math.Truncate(paymentItem.Reward * (_paymentOptions.Tax / 100)) : 0;
            paymentItem.RewardToPay = paymentItem.Reward - paymentItem.Tax;
        }

        public bool TryPay(Payment payment)
        {
            var accountFrom = _paymentOptions.BankAccount;
            if (!payment.IsPaid && payment.PaymentItem != null && payment.PaymentItem.Count() > 0)
            {
                var result = GenerateSummary(payment);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                sb.AppendLine(@"<Import xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""");
                sb.AppendLine(@"xsi:noNamespaceSchemaLocation=""http://www.fio.cz/schema/importIB.xsd"">");
                sb.AppendLine("<Orders>");
                var today = DateTime.Now;
                foreach (var ts in result.Summaries)
                {
                    string ss = "0";
                    if (ts.Person.PaidFrom.Name == "MŠMT")
                        ss = "10";
                    sb.AppendLine("<DomesticTransaction>");
                    sb.AppendLine($"<accountFrom>{accountFrom}</accountFrom>");
                    sb.AppendLine("<currency>CZK</currency>");
                    sb.AppendLine($"<amount>{ts.RewardToPay}</amount>");
                    sb.AppendLine($"<accountTo>{ts.Person.BankAccount}</accountTo>");
                    sb.AppendLine($"<bankCode>{ts.Person.BankCode}</bankCode>");
                    sb.AppendLine($"<ss>{ss}</ss>");
                    sb.AppendLine($"<date>{today.ToString("yyyy-MM-dd")}</date>");
                    sb.AppendLine($"<messageForRecipient>Trenérská odměna {ts.Person.FullName}-{today.AddMonths(-1).Year}/{today.AddMonths(-1).Month}</messageForRecipient>");
                    sb.AppendLine("<paymentType>431001</paymentType>");
                    sb.AppendLine("</DomesticTransaction>");
                }
                sb.AppendLine("</Orders>");
                sb.AppendLine("</Import>");

                payment.PaymentXml = sb.ToString();
                payment.PaymentDateTime = today;
                return true;
            }
            else
            {
                return false;
            }
        }

        public decimal GetPayedAmountInCurrentMonth()
        {
            return DefaultQuery
                    .Where(p => p.PaymentDateTime != null && p.PaymentDateTime.Value.Year == DateTime.Now.Year && p.PaymentDateTime.Value.Month == DateTime.Now.Month)
                    .AsEnumerable()
                    .Select(p => GenerateSummary(p))
                    .Select(p => p.RewardToPay)
                    .Sum();
        }

        public PaymentSummary GenerateSummary(Payment payment)
        {
            var summaries = payment.PaymentItem
                    .Select(t =>
                    {
                        return new RewardSummary()
                        {
                            Person = t.Person,
                            PersonId = t.PersonId,
                            HasTax = t.Person.HasTax,
                            Hours = t.Hours,
                            Reward = t.Reward,
                            Tax = t.Tax,
                        };
                    })
                    .ToList();
            return new PaymentSummary(summaries);
        }
    }

    public class PaymentOptions
    {
        public const string CONFIG_SECTION_NAME = "Payments";
        public string BankAccount { get; set; }
        public decimal Tax { get; set; }
    }
    public class PaymentSummary
    {
        public List<RewardSummary> Summaries { get; set; }
        public decimal Tax { get; private set; }
        public decimal RewardToPay { get; private set; }
        public PaymentSummary(List<RewardSummary> summaries)
        {
            Summaries = summaries ?? new List<RewardSummary>();
            Tax = summaries?.Select(x => x.Tax ?? 0).Sum() ?? 0;
            RewardToPay = summaries?.Select(x => x.RewardToPay ?? 0).Sum() ?? 0;
        }
    }
}
