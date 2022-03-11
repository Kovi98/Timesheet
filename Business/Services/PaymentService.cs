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
        public PaymentService(IOptions<PaymentOptions> paymentOptions, TimesheetContext context) : base(context)
        {
            _paymentOptions = paymentOptions.Value;
        }

        public override async Task<Payment> GetAsync(int id)
        {
            return await _context.Payment
                .Include(x => x.Timesheet)
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
            var summaries = payment.Timesheet
                    .GroupBy(t => t.PersonId)
                    .Select(t =>
                    {
                        var reward = t.Select(x => x.Reward ?? 0).Sum();
                        var person = t.Select(x => x.Person).First();
                        return new RewardSummary()
                        {
                            Person = person,
                            PersonId = t.Key,
                            HasTax = person.HasTax,
                            Hours = t.Select(x => x.Hours ?? 0).Sum(),
                            Reward = reward,
                            Tax = person.HasTax ? Math.Truncate(reward * _paymentOptions.Tax / 100) : 0,
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
        public decimal Tax { get; set; } = 15;
    }
    public class PaymentSummary
    {
        public List<RewardSummary> Summaries { get; set; }
        public decimal Reward { get; private set; }
        public decimal Tax { get; private set; }
        public decimal RewardToPay { get; private set; }
        public PaymentSummary(List<RewardSummary> summaries)
        {
            Summaries = summaries ?? new List<RewardSummary>();
            Reward = summaries?.Select(x => x.Reward ?? 0).Sum() ?? 0;
            Tax = summaries?.Select(x => x.Tax ?? 0).Sum() ?? 0;
            RewardToPay = summaries?.Select(x => x.RewardToPay ?? 0).Sum() ?? 0;
        }
    }
}
