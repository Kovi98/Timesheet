using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Portal.Areas.RewardSummaries.Pages
{
    public class PrintModel : PageModel
    {
        private readonly TimesheetContext _context;

        public PrintModel(TimesheetContext context)
        {
            _context = context;
        }

        public List<Timesheet.Common.Timesheet> Timesheets { get; set; }
        public RewardSummary RewardSummaryDetail { get; set; }

        public async Task OnGetAsync(int year, int month, int personId)
        {
            if (personId == 0)
            {
                if (year == 0)
                {
                    Timesheets = await _context.Timesheet
                        .Include(x => x.Person)
                        .Include(x => x.Payment)
                        .ToListAsync();
                }
                else if (month == 0)
                {
                    Timesheets = await _context.Timesheet
                        .Include(x => x.Person)
                        .Include(x => x.Payment)
                        .Where(x => (x.DateTimeFrom.HasValue ? x.DateTimeFrom.Value.Year : 0) == year)
                        .ToListAsync();
                }
                else
                {
                    Timesheets = await _context.Timesheet
                        .Include(x => x.Person)
                        .Include(x => x.Payment)
                        .Where(x => (x.DateTimeFrom.HasValue ? x.DateTimeFrom.Value.Year : 0) == year && (x.DateTimeFrom.HasValue ? x.DateTimeFrom.Value.Month : 0) == month)
                        .ToListAsync();
                }
            }
            RewardSummaryDetail = new RewardSummary
            {
                Hours = Timesheets.Select(x => x.Hours).Sum(),
                Reward = Timesheets.Select(x => x.Reward).Sum(),
                Tax = Timesheets.Select(x => x.Tax).Sum(),
                CreateDateTimeYear = year,
                CreateDateTimeMonth = month,
                PersonId = personId,
                Person = await _context.Person.FindAsync(personId)
            };
        }
    }
}
