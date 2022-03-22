using Microsoft.EntityFrameworkCore;
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

        public async Task GenerateItemsAndSaveAsync(Payment payment, IEnumerable<int> selectedTimesheetIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
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

                        var paymentItemQuery = _context.PaymentItem.AsNoTracking().Include(x => x.Timesheet).Where(x => x.Id == paymentItemId);
                        var anyTimesheetsInPaymentItem = await paymentItemQuery.Select(x => x.Timesheet.Any()).FirstAsync();

                        if (!anyTimesheetsInPaymentItem)
                        {
                            var paymentItem = await paymentItemQuery.FirstOrDefaultAsync();
                            _context.PaymentItem.Remove(paymentItem);
                            await _context.SaveChangesAsync();
                            _context.Entry(paymentItem).State = EntityState.Detached;
                        }
                        //recalculate
                        else
                        {
                            var paymentItem = await paymentItemQuery.FirstOrDefaultAsync();
                            CalculateRewards(paymentItem);
                            _context.Entry(paymentItem).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            _context.Entry(paymentItem).State = EntityState.Detached;
                        }
                    }
                    //Add
                    foreach (var timesheetId in timesheetsToAdd)
                    {
                        var timesheet = await _context.Timesheet.AsNoTracking().Where(x => x.Id == timesheetId).FirstOrDefaultAsync();
                        var paymentItemQuery = _context.PaymentItem.AsNoTracking().Include(x => x.Timesheet).AsNoTracking().Where(x => x.PaymentId == payment.Id && x.PersonId == timesheet.PersonId && x.Year == timesheet.Year && x.Month == timesheet.Month);
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
                            item.Timesheet.Add(timesheet);

                            CalculateRewards(item);
                            _context.Entry(item).State = EntityState.Added;

                            await _context.SaveChangesAsync();
                        }
                        // recalculate
                        else
                        {

                        }
                        var paymentItemId = paymentItemQuery.Select(x => x.Id).First();
                        timesheet.PaymentItemId = paymentItemId;
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
                            CreateTime = DateTime.Now
                        })
                        .ToList();
                    foreach (var item in payment.PaymentItem)
                    {
                        CalculateRewards(item);
                    }
                }

                await SaveAsync(payment);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
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
            var hasTax = _context.Person.Where(x => x.Id == paymentItem.PersonId).Select(x => x.HasTax).FirstOrDefault();
            paymentItem.Hours = paymentItem.Timesheet.Select(x => x.Hours ?? 0).Sum();
            paymentItem.Reward = paymentItem.Timesheet.Select(x => x.Reward ?? 0).Sum();
            paymentItem.Tax = hasTax ? Math.Truncate(paymentItem.Reward * (_paymentOptions.Tax / 100)) : 0;
            paymentItem.RewardToPay = paymentItem.Reward - paymentItem.Tax;
        }

        public bool TryPay(Payment payment)
        {
            var accountFrom = _paymentOptions.BankAccount;
            if (!payment.IsPaid && payment.PaymentItem != null && payment.PaymentItem.Count() > 0)
            {
                var items = payment.PaymentItem;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                sb.AppendLine(@"<Import xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""");
                sb.AppendLine(@"xsi:noNamespaceSchemaLocation=""http://www.fio.cz/schema/importIB.xsd"">");
                sb.AppendLine("<Orders>");
                var today = DateTime.Now;
                foreach (var item in items)
                {
                    //recalculate rewards to make sure..
                    CalculateRewards(item);
                    _context.Entry(item).State = EntityState.Modified;
                    _context.SaveChanges();
                    _context.Entry(item).State = EntityState.Detached;

                    string ss = "0";
                    if (item.Person.PaidFrom.Name == "MŠMT")
                        ss = "10";
                    sb.AppendLine("<DomesticTransaction>");
                    sb.AppendLine($"<accountFrom>{accountFrom}</accountFrom>");
                    sb.AppendLine("<currency>CZK</currency>");
                    sb.AppendLine($"<amount>{item.RewardToPay}</amount>");
                    sb.AppendLine($"<accountTo>{item.Person.BankAccount}</accountTo>");
                    sb.AppendLine($"<bankCode>{item.Person.BankCode}</bankCode>");
                    sb.AppendLine($"<ss>{ss}</ss>");
                    sb.AppendLine($"<date>{today.ToString("yyyy-MM-dd")}</date>");
                    sb.AppendLine($"<messageForRecipient>Trenérská odměna {item.Person.FullName}-{item.Year}/{item.Month}</messageForRecipient>");
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
                    .Select(p => p.RewardToPay)
                    .Sum();
        }

        public List<Payment> GetLastFive()
        {
            return DefaultQuery
                    .AsNoTracking()
                    .Where(t => t.PaymentDateTime.HasValue).OrderByDescending(t => t.PaymentDateTime).Take(5).ToList();
        }
    }

    public class PaymentOptions
    {
        public const string CONFIG_SECTION_NAME = "Payments";
        public string BankAccount { get; set; }
        public decimal Tax { get; set; }
    }
}
