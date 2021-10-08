using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timesheet.Common
{
    public interface ITimesheetService : IEntityService<Timesheet>
    {
        Task SaveAsync(Timesheet timesheet);
        Task<Timesheet> GetAsync(int id);
        Task RemoveAsync(Timesheet timesheet);
        Task<List<Timesheet>> GetAsync(bool asNoTracking = true);
        Task<List<Timesheet>> GetFreesAsync(bool asNoTracking = true);
        Task<bool> ExistsAsync(int id);
    }
}
