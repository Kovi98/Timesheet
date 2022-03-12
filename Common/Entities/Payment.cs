using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Common
{
    public partial class Payment : IEntity
    {
        public Payment()
        {
            PaymentItem = new HashSet<PaymentItem>();
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
        public virtual ICollection<Timesheet> Timesheet => PaymentItem.SelectMany(x => x.Timesheet).ToList();
        [Display(Name = "Položky platby")]
        public virtual ICollection<PaymentItem> PaymentItem { get; set; }

        //Ručně přidáno
        [Display(Name = "Stav", Description = "Stav platby?")]
        public bool IsPaid { get { return !(PaymentDateTime is null); } }

        [Display(Name = "Odměna", Description = "Hrubá odměna")]
        [DataType(DataType.Currency)]
        public decimal? Reward
        {
            get
            {
                return Timesheet.Select(x => x.Reward ?? 0).Sum();
            }
        }
    }
}
