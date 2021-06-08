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
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IList<RewardSummary> RewardSummary { get;set; }

        public async Task OnGetAsync()
        {
            RewardSummary = await _context.RewardSummary
            .Include(r => r.Person)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth).ToListAsync();
        }
    }
}
