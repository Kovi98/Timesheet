using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Areas.RewardSummaries.Pages
{
    public class PrintModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public PrintModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public List<Timesheet.Entity.Entities.Timesheet> Timesheets { get; set; }
        public RewardSummary RewardSummaryDetail { get; set; }

        public async Task OnGetAsync(int year, int month)
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
            else {
                Timesheets = await _context.Timesheet
                    .Include(x => x.Person)
                    .Include(x => x.Payment)
                    .Where(x => (x.DateTimeFrom.HasValue ? x.DateTimeFrom.Value.Year : 0) == year && (x.DateTimeFrom.HasValue ? x.DateTimeFrom.Value.Month : 0) == month)
                    .ToListAsync();
            }
            RewardSummaryDetail = new RewardSummary
            {
                Hours = Timesheets.Select(x => x.Hours).Sum(),
                Reward = Timesheets.Select(x => x.Reward).Sum(),
                Tax = Timesheets.Select(x => x.Tax).Sum(),

            };
        }
    }
}
