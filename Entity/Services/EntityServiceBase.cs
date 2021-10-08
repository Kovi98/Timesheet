using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Entity.Entities;
using Timesheet.Entity.Interfaces;

namespace Timesheet.Entity.Services
{
    public abstract class EntityServiceBase<T> : IEntityService<T> where T : class, IEntity
    {
        private readonly TimesheetContext _context;
        public EntityServiceBase(TimesheetContext context)
        {
            _context = context;
        }
        public abstract Task<bool> ExistsAsync(int id);

        public abstract Task<T> GetAsync(int id);

        public abstract Task<List<T>> GetAsync(bool asNoTracking = true);

        public async Task RemoveAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync(T entity)
        {
            if (entity.Id > 0)
            {
                SetModified(entity);
            }
            else
            {
                _context.Set<T>().Add(entity);
            }
            await _context.SaveChangesAsync();
        }

        public void SetModified(T entity)
        {
            _context.Entry<T>(entity).State = EntityState.Modified;
        }
    }
}
