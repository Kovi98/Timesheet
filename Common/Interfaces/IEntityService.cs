using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timesheet.Common
{
    public interface IEntityService<T> : IEntityReadonlyService<T> where T : class
    {
        Task SaveAsync(T entity);
        Task RemoveAsync(T entity);
        void SetModified(T entity);
    }
    public interface IEntityReadonlyService<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<List<T>> GetAsync(bool asNoTracking = true);
        Task<bool> ExistsAsync(int id);
    }
}
