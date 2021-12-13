using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class JobService : EntityServiceBase<Job>
    {
        public JobService(TimesheetContext context) : base(context)
        {

        }

        public override async Task<List<Job>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Job.Include(x => x.Person).Include(x => x.Timesheet).AsNoTracking().ToListAsync()
                : await _context.Job.Include(x => x.Person).Include(x => x.Timesheet).ToListAsync();
        }

        public override async Task<Job> GetAsync(int id)
        {
            return await _context.Job
                .Include(x => x.Person)
                .Include(x => x.Timesheet)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
