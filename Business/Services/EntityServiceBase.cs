using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public abstract class EntityServiceBase<T> : EntityReadonlyServiceBase<T>, IEntityService<T> where T : class, IEntity
    {
        public EntityServiceBase(TimesheetContext context) : base(context)
        {
        }

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
    public abstract class EntityReadonlyServiceBase<T> : IEntityReadonlyService<T> where T : class, IEntity
    {
        protected readonly TimesheetContext _context;
        public EntityReadonlyServiceBase(TimesheetContext context)
        {
            _context = context;
        }
        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(x => x.Id == id);
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<List<T>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Set<T>().AsNoTracking().ToListAsync()
                : await _context.Set<T>().ToListAsync();
        }
    }
}
