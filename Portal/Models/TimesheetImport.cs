using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models
{
    public class TimesheetImport
    {
        public bool Success { get; set; }
        public bool CanPassErrors => !(Errors.Contains(TimesheetImportError.DateTimeFromMissing) || Errors.Contains(TimesheetImportError.DateTimeToMissing) || Errors.Contains(TimesheetImportError.JobMissing) || Errors.Contains(TimesheetImportError.PersonMissing) || Errors.Contains(TimesheetImportError.TimesheetNotUnique));
        public Timesheet.Entity.Entities.Timesheet Timesheet { get; set; }
        public IList<TimesheetImportError> Errors { get; set; }
        public TimesheetImport(Timesheet.Entity.Entities.Timesheet timesheet, IList<TimesheetImportError> errors = null)
        {
            Timesheet = timesheet;
            Success = errors == null || errors.Count == 0;
            if (errors == null)
                Errors = new List<TimesheetImportError>();
            else
                Errors = errors;
        }
        public void AddError(TimesheetImportError error)
        {
            if (Success)
                Success = false;
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
