using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models
{
    public class TimesheetImport
    {
        public bool ShouldImport { get; set; }
        public Timesheet.Entity.Entities.Timesheet Timesheet { get; set; }
        public IList<TimesheetImportError> Errors { get; set; }
        public TimesheetImport(Timesheet.Entity.Entities.Timesheet timesheet, IList<TimesheetImportError> errors = null)
        {
            Timesheet = timesheet;
            ShouldImport = errors == null || errors.Count == 0;
            if (errors == null)
                Errors = new List<TimesheetImportError>();
            else
                Errors = errors;
        }
        public void AddError(TimesheetImportError error)
        {
            if (ShouldImport)
                ShouldImport = false;
            Errors.Add(error);
        }

    }

    public enum TimesheetImportError
    {
        JobMissing,
        JobUndefined,
        PersonMissing,
        PersonUndefined,
        TimesheetNotUnique,
        DateTimeFromMissing,
        DateTimeToMissing
    }
}
