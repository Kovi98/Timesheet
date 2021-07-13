using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;
using Microsoft.AspNetCore.Antiforgery;

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
            .Include(r => r.Payment)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth).ToListAsync();
        }
    }
}
