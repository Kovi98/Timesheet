using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common;

namespace Portal.Areas.Timesheets.Pages
{
    public class IndexModel : PageModel, ILoadablePage
    {
        private readonly ITimesheetService _timesheetService;

        private readonly IJobService _jobService;
        private readonly IFinanceService _financeService;
        private readonly IPersonService _personService;

        public IndexModel(ITimesheetService timesheetService, IJobService jobService, IFinanceService financeService, IPersonService personService)
        {
            _timesheetService = timesheetService;
            _jobService = jobService;
            _financeService = financeService;
            _personService = personService;
        }

        public IList<Timesheet.Common.Timesheet> Timesheet { get; set; }
        [BindProperty]
        public Timesheet.Common.Timesheet TimesheetDetail { get; set; }
        public bool IsEditable { get; set; }


        public async Task OnGetNotPayedAsync()
        {
            await LoadData();
            Timesheet = await _timesheetService.GetFreesAsync();
            IsEditable = false;
        }

        public async Task OnGetAsync(int id)
        {
            await LoadData();
            TimesheetDetail = await _timesheetService.GetAsync(id);
            IsEditable = false;
        }

        public async Task OnGetEditAsync(int id)
        {
            await LoadData();
            var timesheet = await _timesheetService.GetAsync(id);
            if ((id > 0 && timesheet != null && !(timesheet.Payment?.IsPaid ?? false)) || (id == 0))
            {
                TimesheetDetail = timesheet;
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

            try
            {
                await _timesheetService.SaveAsync(TimesheetDetail);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _timesheetService.ExistsAsync(TimesheetDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                return await this.PageWithError();
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
                return await this.PageWithError("Nelze smazat výkaz s existující platbou.");
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
        public async Task LoadData()
        {
            Timesheet = await _timesheetService.GetAsync();
            ViewData["JobId"] = new SelectList(await _jobService.GetAsync(), "Id", "Name");
            ViewData["PaidFromId"] = new SelectList(await _financeService.GetAsync(), "Id", "Name");
            ViewData["PersonId"] = new SelectList(await _personService.GetActiveAsync(), "Id", "FullName");
        }
    }
}
