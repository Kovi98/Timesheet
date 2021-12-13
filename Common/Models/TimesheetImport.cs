using System.Collections.Generic;
using Timesheet.Common.Enums;

namespace Timesheet.Common.Models
{
    public class TimesheetImport : ImportBase<Timesheet, TimesheetImportError>
    {
        public TimesheetImport(Timesheet entity, List<TimesheetImportError> errors = null) : base(entity, errors)
        {
        }

        public override bool CanPassErrors => !(Errors.Contains(TimesheetImportError.DateTimeFromMissing) || Errors.Contains(TimesheetImportError.DateTimeToMissing) || Errors.Contains(TimesheetImportError.JobMissing) || Errors.Contains(TimesheetImportError.PersonMissing) || Errors.Contains(TimesheetImportError.TimesheetNotUnique));

    }
}
