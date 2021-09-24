using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Timesheet.DocManager.Entities;
using Timesheet.DocManager.Services;

namespace Portal.Areas.Documents.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class IndexModel : PageModel
    {
        private readonly Timesheet.DocManager.Entities.DocumentContext _context;
        private readonly IDocumentManager _docManager;

        public IndexModel(IDocumentManager documentManager)
        {
            _docManager = documentManager;
        }

        public IList<DocumentStorage> DocumentStorage { get; set; }

        [BindProperty]
        public DocumentStorage DocumentStorageDetail { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Soubor je povinný")]
        [DisplayName("Vložit šablonu...")]
        public IFormFile DocumentUpload { get; set; }

        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            DocumentStorage = await _docManager.GetDocumentsAsync();
            IsEditable = false;
        }
        public async Task OnGetEditAsync()
        {
            DocumentStorage = await _docManager.GetDocumentsAsync();
            DocumentStorageDetail = null;
            IsEditable = true;
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                DocumentStorage = await _docManager.GetDocumentsAsync();
                return Page();
            }

            if (DocumentUpload != null && DocumentUpload.Length > 0 && DocumentUpload.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    DocumentUpload.CopyTo(ms);
                    DocumentStorageDetail.DocumentSource = ms.ToArray();
                }
            }

            try
            {
                await _docManager.SaveAsync(DocumentStorageDetail);
            }
            catch (DbUpdateConcurrencyException)
            {
                DocumentStorage = await _docManager.GetDocumentsAsync();
                return Page();
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
