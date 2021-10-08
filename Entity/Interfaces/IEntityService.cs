using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timesheet.Entity.Interfaces
{
    public interface IEntityService<T> where T : class
    {
        Task SaveAsync(T entity);
        Task<T> GetAsync(int id);
        Task RemoveAsync(T entity);
        Task<List<T>> GetAsync(bool asNoTracking = true);
        Task<bool> ExistsAsync(int id);
        void SetModified(T entity);
    }
}
