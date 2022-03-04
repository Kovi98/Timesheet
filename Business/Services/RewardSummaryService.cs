using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class RewardSummaryService : EntityReadonlyServiceBase<RewardSummary>
    {
        public RewardSummaryService(TimesheetContext context) : base(context)
        {

        }

        public override async Task<RewardSummary> GetAsync(int id)
        {
            return await _context.RewardSummary
            .Include(r => r.Person)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth)
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();
        }

        public override async Task<List<RewardSummary>> GetAsync(bool asNoTracking = true)
        {
            return await _context.RewardSummary
            .Include(r => r.Person)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth).ToListAsync();
        }

        public async Task<List<RewardSummary>> GetAsync(int year = 0, int month = 0, int personId = 0)
        {
            var query = _context.RewardSummary
                .Include(r => r.Person)
                .AsQueryable();
            if (year != 0)
                query = query.Where(x => x.CreateDateTimeYear == year);
            if (month != 0)
                query = query.Where(x => x.CreateDateTimeMonth == month);
            if (personId != 0)
                query = query.Where(x => x.PersonId == personId);
            return await query.ToListAsync();
        }

    }
}
