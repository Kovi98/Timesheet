using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Portal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(ILogger<IndexModel> logger, Timesheet.Entity.Entities.TimesheetContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public IList<Timesheet.Entity.Entities.Timesheet> Timesheet { get; set; }
        [BindProperty]
        public IList<Timesheet.Entity.Entities.Person> Person { get; set; }
        [BindProperty]
        public IList<Timesheet.Entity.Entities.RewardSummary> RewardSummary { get; set; }
        [BindProperty]
        public IList<Timesheet.Entity.Entities.Payment> Payment { get; set; }

        public async Task OnGetAsync()
        {
            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person).ToListAsync();
            Person = await _context.Person
                .Include(p => p.Job)
                .Include(p => p.PayedFrom)
                .Include(p => p.Section).ToListAsync();
            Payment = await _context.Payment
                .Include(p => p.Timesheet).ToListAsync();
            //RewardSummary = await _context.RewardSummary.ToListAsync();
        }
    }
}
