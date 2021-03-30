using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Section
    {
        public Section()
        {
            Person = new HashSet<Person>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Person> Person { get; set; }
    }
}
