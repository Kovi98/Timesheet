using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class RewardSummary
    {
        public int Person { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Reward { get; set; }
        public bool HasTax { get; set; }
        public decimal? Tax { get; set; }
        public int? CreateDateTimeYear { get; set; }
        public int? CreateDateTimeMonth { get; set; }
        public int? Payment { get; set; }
        public DateTime? PaymentDateTime { get; set; }
    }
}
