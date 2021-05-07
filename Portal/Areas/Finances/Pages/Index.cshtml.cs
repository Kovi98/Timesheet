using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Areas.Finances.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IList<Finance> Finance { get;set; }
        [BindProperty]
        public Finance FinanceDetail { get; set; }
        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            Finance = await _context.Finance.Include(x => x.Person).ToListAsync();
            IsEditable = false;
        }
        public async Task OnGetEditAsync(int id)
        {
            Finance = await _context.Finance.Include(x => x.Person).ToListAsync();
            var finance = Finance.FirstOrDefault(t => t.Id == id);
            if (id > 0)
            {
                FinanceDetail = finance;
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

            if (FinanceDetail.Id > 0)
            {
                _context.Attach(FinanceDetail).State = EntityState.Modified;
            }
            else
            {
                FinanceDetail.CreateTime = DateTime.Now;
                _context.Finance.Add(FinanceDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinanceExists(FinanceDetail.Id))
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

            var financeToDelete = await _context.Finance.Include(x => x.Person).FirstOrDefaultAsync(x => x.Id == id);

            if (financeToDelete.Person?.Count == 0)
            {
                return BadRequest("Nelze smazat dotaci, kterou již má vyplněnou trenér.");
            }

            if (financeToDelete != null)
            {
                _context.Finance.Remove(financeToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        private bool FinanceExists(int id)
        {
            return _context.Finance.Any(e => e.Id == id);
        }
    }
}
