using System.Collections.Generic;
using Timesheet.Common.Enums;

namespace Timesheet.Common.Models
{
    public class PersonImport : ImportBase<Person, PersonImportError>
    {
        public PersonImport(Person entity, List<PersonImportError> errors = null) : base(entity, errors)
        {
        }

        public override bool CanPassErrors => throw new System.NotImplementedException();
    }
}
