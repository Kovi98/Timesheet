using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Business;

namespace Portal.Areas.Timesheets.Pages
{
    public class IndexModel : PageModel, ILoadablePage
    {
        private readonly TimesheetService _timesheetService;

        private readonly JobService _jobService;
        private readonly FinanceService _financeService;
        private readonly PersonService _personService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(TimesheetService timesheetService, JobService jobService, FinanceService financeService, PersonService personService, ILogger<IndexModel> logger)
        {
            _timesheetService = timesheetService;
            _jobService = jobService;
            _financeService = financeService;
            _personService = personService;
            _logger = logger;
        }

        public List<Timesheet.Common.Timesheet> Timesheet { get; set; }
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
            if ((id > 0 && timesheet != null && !(timesheet.PaymentItem?.Payment?.IsPaid ?? false)) || (id == 0))
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
            if (!ModelState.IsValid || (TimesheetDetail?.PaymentItem?.Payment?.IsPaid ?? false))
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
            var timesheetToDelete = await _timesheetService.GetAsync(id);

            if (timesheetToDelete == null)
            {
                return NotFound();
            }

            if (timesheetToDelete.PaymentItemId.HasValue && timesheetToDelete.PaymentItemId > 0)
            {
                return BadRequest("Nelze smazat výkaz s existující platbou.");
            }

            await _timesheetService.RemoveAsync(timesheetToDelete);

            return new OkResult();
        }

        public async Task<IActionResult> OnPostDeleteManyAsync(int[] ids)
        {
            List<int> deletedIds = new();

            foreach (var id in ids)
            {
                try
                {
                    var timesheetToDelete = await _timesheetService.GetAsync(id);

                    if (timesheetToDelete == null || (timesheetToDelete.PaymentItemId.HasValue && timesheetToDelete.PaymentItemId > 0))
                        continue;

                    await _timesheetService.RemoveAsync(timesheetToDelete);
                    deletedIds.Add(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return BadRequest("Při odstraňování výkazů nastala chyba");
                }
            }

            return new OkObjectResult(new { Message = $"Bylo smazáno {deletedIds.Count} výkazů práce", DeletedIds = deletedIds });
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
