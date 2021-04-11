using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Timesheet.Entity.Entities;

namespace Portal.Pages.Documents
{
    public class CreateModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public CreateModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DocumentStorage DocumentStorage { get; set; }
        [BindProperty]
        public IFormFile DocumentUpload { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (DocumentUpload != null && DocumentUpload.Length > 0)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    DocumentUpload.CopyTo(stream);
                    DocumentStorage.DocumentSource = stream.ToArray();
                }
            }

            _context.DocumentStorage.Add(DocumentStorage);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
