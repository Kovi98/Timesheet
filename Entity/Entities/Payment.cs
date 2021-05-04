using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "K vyplacení", Description = "Odměna k vyplacení")]
        [DataType(DataType.Currency)]
        public decimal RewardToPay
        {
            get
            {
                decimal value = 0;
                foreach (var item in Timesheet)
                {
                    value += item.Reward ?? 0;
                }
                return value;
            }
        }

        public bool Pay()
        {
            if (!IsPaid && Timesheet != null && Timesheet.Count > 0)
            {
                string xml = (@"<?xml version=""1.0"" encoding=""UTF-8""?>" + Environment.NewLine +
                @"<Import xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" + Environment.NewLine +
                @"xsi:noNamespaceSchemaLocation=""http://www.fio.cz/schema/importIB.xsd"">" + Environment.NewLine +
                "<Orders>" + Environment.NewLine);

                foreach(var ts in Timesheet)
                {
                    string ss = "0";
                    if (ts.Person.PaidFrom.Name == "MŠMT")
                        ss = "10";
                    xml += ("<DomesticTransaction>" + Environment.NewLine +
                    "<accountFrom>2401100335</accountFrom>" + Environment.NewLine +
                    "<currency>CZK</currency>" + Environment.NewLine +
                    "<amount>" + ts.Reward + "</amount>" + Environment.NewLine +
                    "<accountTo>" + ts.Person.BankAccount + "</accountTo>" + Environment.NewLine +
                    "<bankCode>" + ts.Person.BankCode + "</bankCode>" + Environment.NewLine +
                    "<ss>" + ss + "</ss>" + Environment.NewLine +
                    "<date>" + DateTime.Now.ToString("yyyy-MM-dd") + "</date>" + Environment.NewLine +
                    "<messageForRecipient>Trenérská odměna " + ts.Person.FullName + "-" + ts.DateTimeFrom.Value.Year + "/" + ts.DateTimeFrom.Value.Month + "</messageForRecipient>" + Environment.NewLine +
                    "<paymentType>431001</paymentType>" + Environment.NewLine +
                    "</DomesticTransaction>" + Environment.NewLine);
                }
                xml += ("</Orders>" + Environment.NewLine +
                     "</Import>");
                PaymentDateTime = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
