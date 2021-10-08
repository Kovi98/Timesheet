using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class FinanceService : EntityServiceBase<Finance>, IFinanceService
    {
        public FinanceService(TimesheetContext context) : base(context)
        {

        }

        public override async Task<Finance> GetAsync(int id)
        {
            return await _context.Finance
                .Include(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<List<Finance>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Finance.Include(x => x.Person).AsNoTracking().ToListAsync()
                : await _context.Finance.Include(x => x.Person).ToListAsync();
        }
    }
}
