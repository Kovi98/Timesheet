using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Areas.Finances.Pages
{
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
            Finance = await _context.Finance.ToListAsync();
            IsEditable = false;
        }
        public async Task OnGetEditAsync()
        {
            Finance = await _context.Finance.ToListAsync();
            FinanceDetail = null;
            IsEditable = true;
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Finance.Add(FinanceDetail);
            await _context.SaveChangesAsync();

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

            var financeToDelete = await _context.Finance.FindAsync(id);

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
    }
}
