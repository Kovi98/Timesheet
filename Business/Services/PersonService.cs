using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class PersonService : EntityServiceBase<Person>, IPersonService
    {
        public PersonService(TimesheetContext context) : base(context)
        {

        }
        public override Task<Person> GetAsync(int id)
        {
            return _context.Person
                    .Include(p => p.Job)
                    .Include(p => p.PaidFrom)
                    .Include(p => p.Section)
                    .Include(p => p.Timesheet)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
        }

        public async override Task<List<Person>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Person
                    .Include(p => p.Job)
                    .Include(p => p.PaidFrom)
                    .Include(p => p.Section)
                    .Include(p => p.Timesheet)
                    .AsNoTracking().ToListAsync()
                : await _context.Person
                    .Include(p => p.Job)
                    .Include(p => p.PaidFrom)
                    .Include(p => p.Section)
                    .Include(p => p.Timesheet)
                    .ToListAsync();
        }
        public async Task<List<Person>> GetActiveAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Person
                    .Include(p => p.Job)
                    .Include(p => p.PaidFrom)
                    .Include(p => p.Section)
                    .Include(p => p.Timesheet)
                    .Where(p => p.IsActive)
                    .AsNoTracking().ToListAsync()
                : await _context.Person
                    .Include(p => p.Job)
                    .Include(p => p.PaidFrom)
                    .Include(p => p.Section)
                    .Include(p => p.Timesheet)
                    .Where(p => p.IsActive)
                    .ToListAsync();
        }
    }
}
