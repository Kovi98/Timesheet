using System.Collections.Generic;
using Timesheet.Common.Enums;

namespace Timesheet.Common.Models
{
    public class PersonImport : ImportBase<Person, PersonImportError>
    {
        public PersonImport(Person entity, List<PersonImportError> errors = null) : base(entity, errors)
        {
        }

        public override ICollection<PersonImportError> NotPassableErrors => new List<PersonImportError>()
        {
            PersonImportError.NameMissing,
            PersonImportError.SurnameMissing,
            PersonImportError.DateBirthBadFormat,
            PersonImportError.DateBirthMissing,
        };
    }
}
