using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common.Models;

namespace Timesheet.Common.Interfaces
{
    public interface IImportManager
    {
        IList<TimesheetImport> ConvertPeople(byte[] source);
        Task Import(IList<TimesheetImport> import, bool overrideErrors);
    }
}
