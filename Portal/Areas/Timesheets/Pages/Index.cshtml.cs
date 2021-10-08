using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Timesheets.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IList<Timesheet.Entity.Entities.Timesheet> Timesheet { get; set; }
        [BindProperty]
        public Timesheet.Entity.Entities.Timesheet TimesheetDetail { get; set; }
        public bool IsEditable { get; set; }


        public async Task OnGetNotPayedAsync()
        {
            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person)
                .Include(t => t.Person.Section)
                .Include(t => t.Person.PaidFrom)
                .AsNoTracking()
                .ToListAsync();
            Timesheet = Timesheet.Where(t => t.Payment == null || !t.Payment.IsPaid).ToList();
            IsEditable = false;
        }

        public async Task OnGetAsync(int id)
        {
            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person)
                .Include(t => t.Person.Section)
                .Include(t => t.Person.PaidFrom)
                .AsNoTracking()
                .ToListAsync();
            var timesheet = Timesheet.FirstOrDefault(t => t.Id == id);
            if (id > 0 && timesheet != null)
            {
                TimesheetDetail = timesheet;
            }
            IsEditable = false;
        }

        public async Task OnGetEditAsync(int id)
        {
            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person)
                .Include(t => t.Person.Section)
                .Include(t => t.Person.PaidFrom)
                .ToListAsync();
            var timesheet = Timesheet.FirstOrDefault(t => t.Id == id);
            if ((id > 0 && timesheet != null && !(timesheet.Payment?.IsPaid ?? false)) || (id == 0))
            {
                TimesheetDetail = timesheet;
                ViewData["JobId"] = new SelectList(_context.Job, "Id", "Name");
                ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "PaymentDateTime");
                ViewData["PersonId"] = new SelectList(_context.Person.Where(x => x.IsActive), "Id", "FullName");
                IsEditable = true;
            }
            else
            {
                TimesheetDetail = null;
                IsEditable = false;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || (TimesheetDetail?.Payment?.IsPaid ?? false))
            {
                return RedirectToPage("./Index", new { id = TimesheetDetail?.Id, area = "Timesheets" });
            }

            if (!TimesheetDetail.Hours.HasValue && TimesheetDetail.DateTimeFrom != null && TimesheetDetail.DateTimeTo != null)
                TimesheetDetail.Hours = (decimal)(TimesheetDetail.DateTimeTo - TimesheetDetail.DateTimeFrom)?.TotalHours;
            if (!TimesheetDetail.Reward.HasValue)
                TimesheetDetail.Reward = TimesheetDetail.Hours * (_context.Job.Find(TimesheetDetail.JobId).HourReward);
            if (_context.Person.Find(TimesheetDetail.PersonId).HasTax)
            {
                TimesheetDetail.Tax = Math.Truncate((TimesheetDetail.Reward ?? 0) * (decimal)0.15);
            }
            else
            {
                TimesheetDetail.Tax = 0;
            }

            if (TimesheetDetail.Id > 0)
            {
                _context.Attach(TimesheetDetail).State = EntityState.Modified;
            }
            else
            {
                TimesheetDetail.CreateTime = DateTime.Now;
                _context.Timesheet.Add(TimesheetDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimesheetExists(TimesheetDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { id = TimesheetDetail.Id, area = "Timesheets" });
        }

        /// <summary>
        /// Smazání objektu Timesheet
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var timesheetToDelete = await _context.Timesheet.FindAsync(id);

            if (timesheetToDelete != null && timesheetToDelete.PaymentId.HasValue && timesheetToDelete.PaymentId > 0)
            {
                return BadRequest("Nelze smazat výkaz s existující platbou.");
            }

            if (timesheetToDelete != null)
            {
                _context.Timesheet.Remove(timesheetToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }
        private bool TimesheetExists(int id)
        {
            return _context.Timesheet.Any(e => e.Id == id);
        }
    }
}
