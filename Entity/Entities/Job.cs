using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Job
    {
        public Job(string name, decimal hourReward)
        {
            Timesheets = new HashSet<Timesheet>();
            CreateTime = DateTime.Now;

            Name = name;
            HourReward = hourReward;
        }

        #region EF generated
        public Job()
        {
            People = new HashSet<Person>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? HourReward { get; set; }
        public DateTime CreateTime { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Person> People { get; set; }
        public virtual ICollection<Timesheet> Timesheets { get; set; }
        #endregion
    }
}
