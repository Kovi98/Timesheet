using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    /// <summary>
    /// Přehledové view pro zobrazení přehledu výkazů/plateb
    /// </summary>
    public partial class RewardSummary
    {
        #region EF generated
        public int Person { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Reward { get; set; }
        public bool HasTax { get; set; }
        public decimal? Tax { get; set; }
        public int? CreateDateTimeYear { get; set; }
        public int? CreateDateTimeMonth { get; set; }
        public int? Payment { get; set; }
        public DateTime? PaymentDateTime { get; set; }
        #endregion

        public Person PersonObj { get; private set; }
        public Payment PaymentObj { get; private set; }
        public void FillObjects(TimesheetContext context)
        {
            PersonObj = context.People.Find(Person);
            PaymentObj = context.Payments.Find(Payment);
        }
    }
}
