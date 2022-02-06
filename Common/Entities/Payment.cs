using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Common
{
    public partial class Payment : IEntity
    {
        public Payment()
        {
            Timesheet = new HashSet<Timesheet>();
        }

        public int Id { get; set; }

        [Display(Name = "Vytvořeno", Description = "Datum a čas vytvoření záznamu")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "Datum platby", Description = "Datum a čas platby")]
        public DateTime? PaymentDateTime { get; set; }

        [Display(Name = "K platbě", Description = "Určeno k platbě?")]
        public bool ToPay { get; set; }

        [Display(Name = "Platební příkaz")]
        public string PaymentXml { get; set; }
        public byte[] RowVersion { get; set; }

        [Display(Name = "Výkazy práce")]
        public virtual ICollection<Timesheet> Timesheet { get; set; }

        //Ručně přidáno
        [Display(Name = "Stav", Description = "Stav platby?")]
        public bool IsPaid { get { return !(PaymentDateTime is null); } }

        [Display(Name = "Odměna", Description = "Hrubá odměna")]
        [DataType(DataType.Currency)]
        public decimal? Reward
        {
            get
            {
                decimal? value = 0;
                foreach (var item in Timesheet)
                {
                    value += item.Reward;
                }
                return value;
            }
        }
        [Display(Name = "Daň", Description = "Daň")]
        [DataType(DataType.Currency)]
        public decimal Tax
        {
            get
            {
                decimal value = 0;
                foreach (var item in Timesheet)
                {
                    value += item.Tax;
                }
                return value;
            }
        }
        [Display(Name = "K vyplacení", Description = "Odměna k vyplacení")]
        [DataType(DataType.Currency)]
        public decimal RewardToPay
        {
            get
            {
                decimal value = 0;
                foreach (var item in Timesheet)
                {
                    value += item.ToPay;
                }
                return value;
            }
        }

        public bool TryCreatePaymentOrder(string accountFrom)
        {
            if (!IsPaid && Timesheet != null && Timesheet.Count > 0)
            {
                var result = from t in Timesheet
                             group t by t.Person into g
                             select new TimesheetGroup { Person = g.Key, ToPay = g.ToArray().Select(x => x.ToPay).Sum() };

                StringBuilder sb = new StringBuilder();
                //header
                sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                sb.AppendLine(@"<Import xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""");
                sb.AppendLine(@"xsi:noNamespaceSchemaLocation=""http://www.fio.cz/schema/importIB.xsd"">");
                sb.AppendLine("<Orders>");
                var today = DateTime.Now;
                foreach (var ts in result)
                {
                    string ss = "0";
                    if (ts.Person.PaidFrom.Name == "MŠMT")
                        ss = "10";
                    sb.AppendLine("<DomesticTransaction>");
                    sb.AppendLine($"<accountFrom>{accountFrom}</accountFrom>");
                    sb.AppendLine("<currency>CZK</currency>");
                    sb.AppendLine($"<amount>{ts.ToPay}</amount>");
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

                PaymentXml = sb.ToString();
                PaymentDateTime = today;
                return true;
            }
            else
            {
                return false;
            }
        }
        private class TimesheetGroup
        {
            public Person Person { get; set; }
            public decimal ToPay { get; set; }
        }
    }
}
