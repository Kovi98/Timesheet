using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Payment
    {
        #region EF DB first
        public Payment()
        {
            Timesheets = new HashSet<Timesheet>();
        }
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? PaymentDateTime { get; set; }
        public bool ToPay { get; set; }
        public string PaymentXML { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Timesheet> Timesheets { get; set; }
        #endregion

        public bool IsPayed { get { return !(PaymentDateTime is null); } }
    }
}
