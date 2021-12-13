using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Business;
using Timesheet.Common;

namespace Portal.Areas.Jobs.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class IndexModel : PageModel, ILoadablePage
    {
        private readonly JobService _jobService;

        public IndexModel(JobService jobService)
        {
            _jobService = jobService;
        }

        public List<Job> Job { get; set; }
        [BindProperty]
        public Job JobDetail { get; set; }
        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            await LoadData();
            IsEditable = false;
        }
        public async Task OnGetEditAsync(int id)
        {
            await LoadData();
            if (id > 0)
            {
                JobDetail = Job.FirstOrDefault(t => t.Id == id);
            }
            else
            {
                JobDetail = null;
            }
            IsEditable = true;
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Index", new { id = JobDetail?.Id, area = "Jobs" });
            }

            try
            {
                await _jobService.SaveAsync(JobDetail);
            }
            catch (DbUpdateConcurrencyException)
            {
                await LoadData();
                if (JobDetail.Id > 0)
                {
                    JobDetail = Job.FirstOrDefault(t => t.Id == JobDetail.Id);
                }
                else
                {
                    JobDetail = null;
                }
                IsEditable = true;
                ModelState.AddModelError("Error", "Tento záznam byl změněn jiným uživatelem. Aktualizujte si záznam.");
                return Page();
            }
            catch (Exception)
            {
                return await this.PageWithError();
            }
            return RedirectToPage("Index");
        }

        /// <summary>
        /// Smazání objektu Job
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var JobToDelete = await _jobService.GetAsync(id);

            if (JobToDelete.Person?.Count != 0)
            {
                return await this.PageWithError("Nelze smazat pozici, kterou již má vyplněnou trenér.");

            }

            if (JobToDelete.Timesheet.Count != 0)
            {
                return await this.PageWithError("Nelze smazat pozici, kterou již má vyplněnou výkaz práce.");
            }

            if (JobToDelete != null)
            {
                await _jobService.RemoveAsync(JobToDelete);
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        public async Task LoadData()
        {
            Job = await _jobService.GetAsync();
        }
    }
}
