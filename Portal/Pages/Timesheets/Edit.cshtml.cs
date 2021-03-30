using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Pages.Timesheets
{
    public class EditModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public EditModel(Timesheet.Entity.Entities.TimesheetContext context)
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
           ViewData["JobId"] = new SelectList(_context.Job, "Id", "Name");
           ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "Id");
           ViewData["PersonId"] = new SelectList(_context.Person, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Timesheet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimesheetExists(Timesheet.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TimesheetExists(int id)
        {
            return _context.Timesheet.Any(e => e.Id == id);
        }
    }
}
