using System.Collections.Generic;
using Timesheet.Common.Enums;

namespace Timesheet.Common.Models
{
    public class TimesheetImport : ImportBase<Timesheet, TimesheetImportError>
    {
        public TimesheetImport(Timesheet entity, List<TimesheetImportError> errors = null) : base(entity, errors)
        {
        }

        public override ICollection<TimesheetImportError> NotPassableErrors => new[]
        {
            TimesheetImportError.DateTimeFromMissing,
            TimesheetImportError.DateTimeToMissing,
            TimesheetImportError.JobMissing,
            TimesheetImportError.PersonMissing,
            TimesheetImportError.TimesheetNotUnique
        };
    }
}
