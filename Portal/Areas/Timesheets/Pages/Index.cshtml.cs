using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;

namespace Portal.Areas.Timesheets.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITimesheetService _timesheetService;

        public IndexModel(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }

        public IList<Timesheet.Common.Timesheet> Timesheet { get; set; }
        [BindProperty]
        public Timesheet.Common.Timesheet TimesheetDetail { get; set; }
        public bool IsEditable { get; set; }


        public async Task OnGetNotPayedAsync()
        {
            await LoadData();
            Timesheet = Timesheet.Where(t => t.Payment == null || !t.Payment.IsPaid).ToList();
            IsEditable = false;
        }

        public async Task OnGetAsync(int id)
        {
            await LoadData();
            var timesheet = await _timesheetService.GetAsync(id);
            if (id > 0 && timesheet != null)
            {
                TimesheetDetail = timesheet;
            }
            IsEditable = false;
        }

        public async Task OnGetEditAsync(int id)
        {
            LoadData();
            var timesheet = await _timesheetService.GetAsync(id);
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
            if (!await _timesheetService.ExistsAsync(id))
            {
                return NotFound();
            }

            var timesheetToDelete = await _timesheetService.GetAsync(id);

            if (timesheetToDelete != null && timesheetToDelete.PaymentId.HasValue && timesheetToDelete.PaymentId > 0)
            {
                return BadRequest("Nelze smazat výkaz s existující platbou.");
            }

            if (timesheetToDelete != null)
            {
                await _timesheetService.RemoveAsync(timesheetToDelete);
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }
        private async Task LoadData()
        {
            Timesheet = await _timesheetService.GetAsync();
        }
    }
}
