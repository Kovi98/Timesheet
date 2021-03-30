using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Pages.Timesheets
{
    public class DeleteModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public DeleteModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Timesheet.Entity.Entities.Timesheet Timesheet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person).FirstOrDefaultAsync(m => m.Id == id);

            if (Timesheet == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Timesheet = await _context.Timesheet.FindAsync(id);

            if (Timesheet != null)
            {
                _context.Timesheet.Remove(Timesheet);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
