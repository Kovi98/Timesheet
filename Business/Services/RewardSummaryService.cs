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
            .Include(r => r.Payment)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth)
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();
        }

        public override async Task<List<RewardSummary>> GetAsync(bool asNoTracking = true)
        {
            return await _context.RewardSummary
            .Include(r => r.Person)
            .Include(r => r.Payment)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth).ToListAsync();
        }
    }
}
