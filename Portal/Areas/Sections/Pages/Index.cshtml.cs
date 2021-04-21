using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Areas.Sections.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IList<Section> Section { get; set; }
        [BindProperty]
        public Section SectionDetail { get; set; }
        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            Section = await _context.Section.ToListAsync();
            IsEditable = false;
        }
        public async Task OnGetEditAsync(int id)
        {
            Section = await _context.Section.ToListAsync();
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

            if (SectionDetail.Id > 0)
            {
                _context.Attach(SectionDetail).State = EntityState.Modified;
            }
            else
            {
                SectionDetail.CreateTime = DateTime.Now;
                _context.Section.Add(SectionDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SectionExists(SectionDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
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

            var sectionToDelete = await _context.Section.FindAsync(id);

            if (false)
            {
                //Existujicí výkaz
            }

            if (sectionToDelete != null)
            {
                _context.Section.Remove(sectionToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        private bool SectionExists(int id)
        {
            return _context.Section.Any(e => e.Id == id);
        }
    }
}
