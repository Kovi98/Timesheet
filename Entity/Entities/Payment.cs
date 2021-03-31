using System;
using System.Collections.Generic;

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
        public DateTime CreateTime { get; set; }
        public DateTime? PaymentDateTime { get; set; }
        public bool ToPay { get; set; }
        public string PaymentXml { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Timesheet> Timesheet { get; set; }
    }
}
