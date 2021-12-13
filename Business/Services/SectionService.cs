using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class SectionService : EntityServiceBase<Section>
    {
        public SectionService(TimesheetContext context) : base(context)
        {

        }

        public override Task<Section> GetAsync(int id)
        {
            return _context.Section.Include(x => x.Person).FirstOrDefaultAsync(x => x.Id == id);
        }

        public override Task<List<Section>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.Section.Include(x => x.Person).AsNoTracking().ToListAsync()
                : _context.Section.Include(x => x.Person).ToListAsync();
        }
    }
}
