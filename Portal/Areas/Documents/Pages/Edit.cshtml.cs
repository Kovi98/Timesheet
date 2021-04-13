using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Timesheet.DocManager.Entities;

namespace Portal.Areas.Documents.Pages
{
    public class EditModel : PageModel
    {
        private readonly Timesheet.DocManager.Entities.DocumentContext _context;

        public EditModel(Timesheet.DocManager.Entities.DocumentContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(DocumentStorage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentStorageExists(DocumentStorage.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DocumentStorageExists(int id)
        {
            return _context.DocumentStorage.Any(e => e.Id == id);
        }
    }
}
