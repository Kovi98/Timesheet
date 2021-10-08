using System.Collections.Generic;
using Timesheet.Common.Models;

namespace Timesheet.Common.Interfaces
{
    public interface IImportManager
    {
        IList<TimesheetImport> ConvertPeople(byte[] source);
    }
}
