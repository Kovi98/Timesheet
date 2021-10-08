using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Common
{
    public partial class Section : IEntity
    {
        public Section()
        {
            Person = new HashSet<Person>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný")]
        [Display(Name = "Název")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string Name { get; set; }

        [Display(Name = "Vytvořeno", Description = "Datum a čas vytvoření záznamu")]
        public DateTime CreateTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Person> Person { get; set; }
    }
}
