using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Common
{
    public partial class RewardSummary : IEntity
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Reward { get; set; }
        public bool HasTax { get; set; }
        public decimal? Tax { get; set; }
        public int? CreateDateTimeYear { get; set; }
        public int? CreateDateTimeMonth { get; set; }
        public int? PaymentId { get; set; }
        public DateTime? PaymentDateTime { get; set; }

        public Person Person { get; set; }
        public Payment Payment { get; set; }

        public decimal? RewardToPay { get => Reward - Tax; }
        //public virtual IList<Timesheet> Timesheet { get; set; }
    }
}
