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
        private readonly PaymentOptions _paymentOptions;
        private readonly TimesheetService _timesheetService;
        public PaymentService(IOptions<PaymentOptions> paymentOptions, TimesheetContext context, TimesheetService timesheetService) : base(context)
        {
            _paymentOptions = paymentOptions.Value;
            _timesheetService = timesheetService;
        }

        public override async Task<Payment> GetAsync(int id)
        {
            return await _context.Payment
                .Include(x => x.PaymentItem)
                    .ThenInclude(x => x.Person)
                    .ThenInclude(x => x.PaidFrom)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<List<Payment>> GetAsync(bool asNoTracking = true)
        {
            var payments = _context
                .Payment
                .Include(x => x.Timesheet)
                .ThenInclude(x => x.Person)
                .ThenInclude(x => x.PaidFrom);
            return await (asNoTracking ? payments.AsNoTracking().ToListAsync() : payments.ToListAsync());
        }

        public async Task GenerateItemsAsync(Payment payment, IEnumerable<int> selectedTimesheetIds)
        {
            if (payment.Id > 0)
            {
                var oldTimesheets = await _timesheetService.GetPaymentTimesheetsAsync(payment.Id, false);
                var timesheetsToRemove = new List<Timesheet.Common.Timesheet>();
                var timesheetsToAdd = new List<Timesheet.Common.Timesheet>();
                foreach (var timesheet in oldTimesheets)
                {
                    if (!selectedTimesheetIds.Any(x => x == timesheet.Id))
                    {
                        timesheetsToRemove.Add(timesheet);
                    }
                }

                foreach (var id in selectedTimesheetIds)
                {
                    if (oldTimesheets.Any(x => x.Id != id))
                        timesheetsToAdd.Add(await _timesheetService.GetAsync(id));
                }

                //Remove
                foreach (var timesheet in timesheetsToRemove)
                {
                    timesheet.PaymentItem = null;
                    timesheet.PaymentItemId = null;
                    await _timesheetService.SaveAsync(timesheet);
                }
                //Add
                foreach (var timesheet in timesheetsToAdd)
                {
                    var paymentItemQuery = _context.PaymentItem.Include(x => x.Timesheet).AsNoTracking().Where(x => x.PaymentId == payment.Id && x.PersonId == timesheet.PersonId && x.Year == timesheet.Year && x.Month == timesheet.Month);
                    //create new one
                    if (!paymentItemQuery.Any())
                    {
                        var item = new PaymentItem()
                        {
                            PaymentId = payment.Id,
                            PersonId = timesheet.PersonId,
                            Year = timesheet.Year,
                            Month = timesheet.Month
                        };
                        _context.PaymentItem.Add(item);
                        await _context.SaveChangesAsync();
                    }
                    var paymentItem = paymentItemQuery.First();
                    paymentItem.Timesheet.Add(timesheet);
                    timesheet.PaymentItemId = paymentItem.Id;
                    timesheet.PaymentItem = paymentItem;
                    await _timesheetService.SaveAsync(timesheet);
                }
            }
            else
            {
                var timesheets = selectedTimesheetIds.Select(async x => await _timesheetService.GetAsync(x)).Select(x => x.GetAwaiter().GetResult());
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
                        Person = _context.Person.AsNoTracking().Where(y => y.Id == x.Key.PersonId).FirstOrDefault()
                    })
                    .ToList();
            }
        }

        public bool TryPay(Payment payment)
        {
            var accountFrom = _paymentOptions.BankAccount;
            if (!payment.IsPaid && payment.Timesheet != null && payment.Timesheet.Count > 0)
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
            return _context.Payment
                .Include(x => x.Timesheet)
                    .ThenInclude(x => x.Person)
                    .ThenInclude(x => x.PaidFrom)
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
