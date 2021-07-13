using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Payment
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
            if (true)//!IsPaid && Timesheet != null && Timesheet.Count > 0)
            {
                var result = from t in Timesheet
                             group t by t.Person into g
                             select new TimesheetGroup { Person = g.Key, ToPay = g.ToArray().Select(x => x.ToPay).Sum() };
                string xml = (@"<?xml version=""1.0"" encoding=""UTF-8""?>" + Environment.NewLine +
                @"<Import xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" + Environment.NewLine +
                @"xsi:noNamespaceSchemaLocation=""http://www.fio.cz/schema/importIB.xsd"">" + Environment.NewLine +
                "<Orders>" + Environment.NewLine);
                var today = DateTime.Now;
                foreach (var ts in result)
                {
                    string ss = "0";
                    if (ts.Person.PaidFrom.Name == "MŠMT")
                        ss = "10";
                    xml += ("<DomesticTransaction>" + Environment.NewLine +
                    "<accountFrom>" + accountFrom + "</accountFrom>" + Environment.NewLine +
                    "<currency>CZK</currency>" + Environment.NewLine +
                    "<amount>" + ts.ToPay + "</amount>" + Environment.NewLine +
                    "<accountTo>" + ts.Person.BankAccount + "</accountTo>" + Environment.NewLine +
                    "<bankCode>" + ts.Person.BankCode + "</bankCode>" + Environment.NewLine +
                    "<ss>" + ss + "</ss>" + Environment.NewLine +
                    "<date>" + today.ToString("yyyy-MM-dd") + "</date>" + Environment.NewLine +
                    "<messageForRecipient>Trenérská odměna " + ts.Person.FullName + "-" + today.AddMonths(-1).Year + "/" + today.AddMonths(-1).Month + "</messageForRecipient>" + Environment.NewLine +
                    "<paymentType>431001</paymentType>" + Environment.NewLine +
                    "</DomesticTransaction>" + Environment.NewLine);
                }
                xml += ("</Orders>" + Environment.NewLine +
                     "</Import>");
                PaymentXml = xml;
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
