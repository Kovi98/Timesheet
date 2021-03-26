using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Job
    {
        public Job()
        {
            People = new HashSet<Person>();
            Timesheets = new HashSet<Timesheet>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? HourReward { get; set; }
        public DateTime CreateTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Person> People { get; set; }
        public virtual ICollection<Timesheet> Timesheets { get; set; }
    }
}
