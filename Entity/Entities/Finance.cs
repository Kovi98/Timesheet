using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Finance
    {
        public Finance()
        {
            People = new HashSet<Person>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Person> People { get; set; }
    }
}
