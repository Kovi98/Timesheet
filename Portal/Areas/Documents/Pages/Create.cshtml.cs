using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Timesheet.DocManager.Entities;

namespace Portal.Areas.Documents.Pages
{
    public class CreateModel : PageModel
    {
        private readonly Timesheet.DocManager.Entities.DocumentContext _context;

        public CreateModel(Timesheet.DocManager.Entities.DocumentContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DocumentStorage DocumentStorage { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DocumentStorage.Add(DocumentStorage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
