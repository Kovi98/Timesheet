using System;
using System.ComponentModel.DataAnnotations;
using Timesheet.Common.Attributes;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Common
{
    public partial class Timesheet : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Hodiny", Description = "Vykázaný čas")]
        [RegularExpression(@"^[0-9]\d{0,3}(\,\d{1,2})?%?$", ErrorMessage = "Pole {0} musí být desetinné číslo")]
        public decimal? Hours { get; set; }

        [Display(Name = "Trenér")]
        [Required(ErrorMessage = "Trenér je povinný")]
        public int PersonId { get; set; }
        [Display(Name = "Práce")]
        [Required(ErrorMessage = "Práce je povinná")]
        public int JobId { get; set; }

        [Display(Name = "Vytvořeno", Description = "Datum a čas vytvoření záznamu")]
        public DateTime CreateTime { get; set; }

        public byte[] RowVersion { get; set; }

        [Display(Name = "Od", Description = "Začátek")]
        public DateTime? DateTimeFrom { get; set; }

        [Display(Name = "Do", Description = "Konec")]
        [TimesheetDateTimeRange(nameof(DateTimeFrom))]
        public DateTime? DateTimeTo { get; set; }

        [Required(ErrorMessage = "Název je povinný")]
        [Display(Name = "Text")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string Name { get; set; }


        [Display(Name = "Odměna", Description = "Odměna za práci")]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^[0-9]\d{0,16}(\,\d{1,2})?%?$", ErrorMessage = "Pole {0} musí být desetinné číslo")]
        public decimal? Reward { get; set; }


        public virtual Job Job { get; set; }
        public virtual PaymentItem PaymentItem { get; set; }
        public int? PaymentItemId { get; set; }
        public virtual Person Person { get; set; }

        //Ručně přidáno

        [Display(Name = "Hodiny", Description = "Vykázaný čas")]
        public TimeSpan HoursTime
        {
            get
            {
                return TimeSpan.FromHours(Hours.HasValue ? (double)Hours.Value : 0);
            }
        }

        public string FriendlyName
        {
            get
            {
                return this.ToString();
            }
        }

        public override string ToString() => $"{Person?.FullName ?? "Nevyplněno"} ({DateTimeFrom?.ToString("dd.MM.yyyy HH:mm") ?? "Nevyplněno"} - {DateTimeTo?.ToString("dd.MM.yyyy HH:mm") ?? "Nevyplněno"})";

        public int? Year => DateTimeTo?.Year;
        public int? Month => DateTimeTo?.Month;
    }
}
