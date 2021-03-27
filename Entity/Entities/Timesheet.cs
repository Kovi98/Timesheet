using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Timesheet
    {
        #region EF generated
        public Timesheet()
        {

        }
        public int Id { get; set; }
        public decimal? Hours { get; set; }
        public int PersonId { get; set; }
        public int JobId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }
        public string Name { get; set; }
        public int? PaymentId { get; set; }
        public decimal? Reward { get; set; }
        public decimal Tax { get; set; }

        public virtual Job Job { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Person Person { get; set; }
        #endregion

        public Timesheet(Person person, DateTime dateTimeFrom, DateTime dateTimeTo, Job job, string name)
        {
            CreateDateTime = DateTime.Now;

            DateTimeFrom = dateTimeFrom;
            DateTimeTo = dateTimeTo;
            Person = person;
            Job = job;
            Name = name;
        }
    }
}
