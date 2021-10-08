using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;

namespace Portal.Areas.Finances.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IFinanceService _financeService;

        public IndexModel(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public IList<Finance> Finance { get; set; }
        [BindProperty]
        public Finance FinanceDetail { get; set; }
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
                FinanceDetail = Finance.FirstOrDefault(t => t.Id == id);
            }
            else
            {
                FinanceDetail = null;
            }
            IsEditable = true;
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Index", new { id = FinanceDetail?.Id, area = "Finances" });
            }

            try
            {
                await _financeService.SaveAsync(FinanceDetail);
            }
            catch (DbUpdateConcurrencyException)
            {
                await LoadData();
                if (FinanceDetail.Id > 0)
                {
                    FinanceDetail = Finance.FirstOrDefault(t => t.Id == FinanceDetail.Id);
                }
                else
                {
                    FinanceDetail = null;
                }
                IsEditable = true;
                ModelState.AddModelError("Error", "Tento záznam byl změněn jiným uživatelem. Aktualizujte si záznam.");
                return Page();
            }
            return RedirectToPage("Index");
        }

        /// <summary>
        /// Smazání objektu Finance
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var financeToDelete = await _financeService.GetAsync(id);

            if (financeToDelete.Person?.Count != 0)
            {
                return BadRequest("Nelze smazat dotaci, kterou již má vyplněnou trenér.");
            }

            if (financeToDelete != null)
            {
                await _financeService.RemoveAsync(financeToDelete);
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        private async Task LoadData()
        {
            Finance = await _financeService.GetAsync();
        }
    }
}
