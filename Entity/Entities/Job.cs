using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Job
    {
        public Job()
        {
            Person = new HashSet<Person>();
            Timesheet = new HashSet<Timesheet>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný")]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [RegularExpression(@"^[0-9]\d{0,16}(\,\d{1,2})?%?$", ErrorMessage = "Pole {0} musí být desetinné číslo")]
        [Display(Name = "Hodinová sazba", Description = "Odměna za hodinu práce")]
        [DataType(DataType.Currency)]
        public decimal? HourReward { get; set; }

        [Display(Name = "Vytvořeno", Description = "Datum a čas vytvoření záznamu")]
        public DateTime CreateTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Person> Person { get; set; }
        public virtual ICollection<Timesheet> Timesheet { get; set; }
    }
}
