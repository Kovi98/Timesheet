using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;

namespace Portal.Areas.Sections.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class IndexModel : PageModel
    {
        private readonly ISectionService _sectionService;

        public IndexModel(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        public IList<Section> Section { get; set; }
        [BindProperty]
        public Section SectionDetail { get; set; }
        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            await LoadData();
            IsEditable = false;
        }
        public async Task OnGetEditAsync(int id)
        {
            await LoadData();
            var section = Section.FirstOrDefault(t => t.Id == id);
            if (id > 0)
            {
                SectionDetail = section;
            }
            else
            {
                SectionDetail = null;
            }
            IsEditable = true;
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Index", new { id = SectionDetail?.Id, area = "Sections" });
            }

            try
            {
                await _sectionService.SaveAsync(SectionDetail);
            }
            catch (DbUpdateConcurrencyException)
            {
                await LoadData();
                if (SectionDetail.Id > 0)
                {
                    var section = Section.FirstOrDefault(t => t.Id == SectionDetail.Id);
                    SectionDetail = section;
                }
                else
                {
                    SectionDetail = null;
                }
                IsEditable = true;
                ModelState.AddModelError("Error", "Tento záznam byl změněn jiným uživatelem. Aktualizujte si záznam.");
                return Page();
            }
            return RedirectToPage("Index");
        }

        /// <summary>
        /// Smazání objektu Section
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var sectionToDelete = await _sectionService.GetAsync(id);

            if (sectionToDelete?.Person?.Count != 0)
            {
                return BadRequest("Nelze smazat sekci, kterou již má vyplněnou trenér.");
            }

            if (sectionToDelete != null)
            {
                await _sectionService.RemoveAsync(sectionToDelete);
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        private async Task LoadData()
        {
            Section = await _sectionService.GetAsync();
        }
    }
}
