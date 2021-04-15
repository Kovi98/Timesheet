using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Zaplaceno", Description = "Je platba zaplacena?")]
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
    }
}
