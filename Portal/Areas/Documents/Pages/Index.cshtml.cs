using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Timesheet.Common;

namespace Portal.Areas.Documents.Pages
{
    [Authorize(Policy = "AdminPolicy")]
    public class IndexModel : PageModel, ILoadablePage
    {
        private readonly IDocumentManager _docManager;
        private readonly ILogger<IDocumentManager> _logger;
        private readonly string _errorText = $"Error in {typeof(IndexModel).Namespace} : {typeof(IndexModel).FullName}";

        public IndexModel(IDocumentManager documentManager, ILogger<IDocumentManager> logger)
        {
            _docManager = documentManager;
            _logger = logger;
        }

        public List<DocumentStorage> DocumentStorage { get; set; }

        [BindProperty]
        public DocumentStorage DocumentStorageDetail { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Soubor je povinný")]
        [DisplayName("Vložit šablonu...")]
        public IFormFile DocumentUpload { get; set; }

        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                await LoadData();
                IsEditable = false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, _errorText);
            }
        }
        public async Task OnGetEditAsync()
        {
            try
            {
                await LoadData();
                DocumentStorageDetail = null;
                IsEditable = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, _errorText);
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (DocumentUpload != null && DocumentUpload.Length > 0 && DocumentUpload.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    using MemoryStream ms = new MemoryStream();
                    DocumentUpload.CopyTo(ms);
                    DocumentStorageDetail.DocumentSource = ms.ToArray();
                    DocumentStorageDetail.DocumentName = DocumentUpload.FileName;
                }

                await _docManager.SaveAsync(DocumentStorageDetail);
            }
            catch (DbUpdateConcurrencyException)
            {
                await LoadData();
                return Page();
            }
            catch (Exception e)
            {
                _logger.LogError(e, _errorText);
                return await this.PageWithError();
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
            if (id == 0) return NotFound();

            var documentStorageToDelete = await _docManager.GetAsync(id);

            try
            {
                if (documentStorageToDelete != null)
                {
                    await _docManager.RemoveAsync(documentStorageToDelete);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, _errorText);
                return await this.PageWithError();
            }

            return new OkResult();
        }

        public async Task<IActionResult> OnPostDownloadDocument(int id, string returnUrl = null)
        {
            var document = await _docManager.GetAsync(id);
            if (document is null) return LocalRedirect(returnUrl);

            return File(document.DocumentSource, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", document.DocumentName);
        }

        public async Task LoadData()
        {
            DocumentStorage = await _docManager.GetDocumentsAsync();
        }
    }
}
