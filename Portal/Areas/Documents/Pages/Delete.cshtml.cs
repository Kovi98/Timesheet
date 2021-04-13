using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.DocManager.Entities;

namespace Portal.Areas.Documents.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly Timesheet.DocManager.Entities.DocumentContext _context;

        public DeleteModel(Timesheet.DocManager.Entities.DocumentContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DocumentStorage DocumentStorage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DocumentStorage = await _context.DocumentStorage.FirstOrDefaultAsync(m => m.Id == id);

            if (DocumentStorage == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DocumentStorage = await _context.DocumentStorage.FindAsync(id);

            if (DocumentStorage != null)
            {
                _context.DocumentStorage.Remove(DocumentStorage);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
