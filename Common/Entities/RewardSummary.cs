// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

using System.ComponentModel.DataAnnotations;

namespace Timesheet.Common
{
    public partial class RewardSummary : IEntityView
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public decimal? Hours { get; set; }
        [DataType(DataType.Currency)]
        public decimal? Reward { get; set; }
        [DataType(DataType.Currency)]
        public decimal? Tax { get; set; }
        [DataType(DataType.Currency)]
        public decimal? RewardToPay { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }

        public Person Person { get; set; }
    }
}
