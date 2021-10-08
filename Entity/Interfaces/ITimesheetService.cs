using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timesheet.Entity.Interfaces
{
    public interface ITimesheetService : IEntityService<Entities.Timesheet>
    {
        Task SaveAsync(Entities.Timesheet timesheet);
        Task<Entities.Timesheet> GetAsync(int id);
        Task RemoveAsync(Entities.Timesheet timesheet);
        Task<List<Entities.Timesheet>> GetAsync(bool asNoTracking = true);
        Task<List<Entities.Timesheet>> GetFreesAsync(bool asNoTracking = true);
        Task<bool> ExistsAsync(int id);
    }
}
