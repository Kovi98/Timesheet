using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timesheet.Common
{
    public interface IPersonService : IEntityService<Person>
    {
        Task<List<Person>> GetActiveAsync(bool asNoTracking = true);
    }
}
