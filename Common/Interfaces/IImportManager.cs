using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common.Models;

namespace Timesheet.Common.Interfaces
{
    public interface IImportManager
    {
        List<TimesheetImport> ConvertTimesheets(byte[] source);
        List<PersonImport> ConvertPeople(byte[] source);
        Task<int> Import(List<TimesheetImport> import, bool overrideErrors);
        Task<int> Import(List<PersonImport> imports, bool overrideErrors);
    }
}
