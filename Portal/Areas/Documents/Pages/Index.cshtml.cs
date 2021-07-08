using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.DocManager.Entities;

namespace Portal.Areas.Documents.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class IndexModel : PageModel
    {
        private readonly Timesheet.DocManager.Entities.DocumentContext _context;

        public IndexModel(Timesheet.DocManager.Entities.DocumentContext context)
        {
            _context = context;
        }

        public IList<DocumentStorage> DocumentStorage { get;set; }

        [BindProperty]
        public DocumentStorage DocumentStorageDetail { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Soubor je povinný")]
        [DisplayName("Vložit šablonu...")]
        public IFormFile DocumentUpload { get; set; }

        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            DocumentStorage = await _context.DocumentStorage.ToListAsync();
            IsEditable = false;
        }
        public async Task OnGetEditAsync()
        {
            DocumentStorage = await _context.DocumentStorage.ToListAsync();
            DocumentStorageDetail = null;
            IsEditable = true;
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!(DocumentUpload is null) && DocumentUpload.Length > 0 && DocumentUpload.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    DocumentUpload.CopyTo(ms);
                    DocumentStorageDetail.DocumentSource = ms.ToArray();
                }
            }
            
            if (!ModelState.IsValid)
            {
                DocumentStorage = await _context.DocumentStorage.ToListAsync();
                return Page();
            }
            if (DocumentStorageDetail.Id > 0)
            {
                DocumentStorage = await _context.DocumentStorage.ToListAsync();
                return Page();
            }
            else
            {
                DocumentStorageDetail.CreateTime = DateTime.Now;
                _context.DocumentStorage.Add(DocumentStorageDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.DocumentStorage.Any(x => x.Id == DocumentStorageDetail.Id))
                {
                    return NotFound();
                }
            }

            return RedirectToPage("Index");
        }

        /// <summary>
        /// Smazání objektu DocumentStorage
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var documentStorageToDelete = await _context.DocumentStorage.FindAsync(id);

            if (documentStorageToDelete != null)
            {
                _context.DocumentStorage.Remove(documentStorageToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        public async Task<IActionResult> OnPostDownloadDocument(int id, string returnUrl = null)
        {
            var document = await _context.DocumentStorage.FindAsync(id);
            if (document is null)
            {
                return LocalRedirect(returnUrl);
            }

            return File(document.DocumentSource, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", document.DocumentName);
        }
    }
}
