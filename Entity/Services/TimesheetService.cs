using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Entity.Entities;
using Timesheet.Entity.Interfaces;

namespace Timesheet.Entity.Services
{
    public class TimesheetService : EntityServiceBase<Entities.Timesheet>, ITimesheetService
    {
        private readonly TimesheetContext _context;
        public TimesheetService(TimesheetContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<bool> ExistsAsync(int id)
        {
            return await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person)
                .Include(t => t.Person.Section)
                .Include(t => t.Person.PaidFrom)
                .AnyAsync(x => x.Id == id);
        }

        public override async Task<Entities.Timesheet> GetAsync(int id)
        {
            return await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person)
                .Include(t => t.Person.Section)
                .Include(t => t.Person.PaidFrom)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public override async Task<List<Entities.Timesheet>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .AsNoTracking().ToListAsync()
                : await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .ToListAsync();
        }

        public async Task<List<Entities.Timesheet>> GetFreesAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? (await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => (x.PaymentId == 0 || x.PaymentId == null))
                    .AsNoTracking().ToListAsync())
                : (await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => (x.PaymentId == 0 || x.PaymentId == null))
                    .AsNoTracking().ToListAsync());
        }
    }
}
