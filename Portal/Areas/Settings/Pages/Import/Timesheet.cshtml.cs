using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common.Interfaces;
using Timesheet.Common.Models;

namespace Portal.Areas.Settings.Pages.Import
{
    [Authorize(Policy = "AdminPolicy")]
    public class TimesheetModel : PageModel
    {
        private readonly IImportManager _importManager;
        private readonly ILogger<IImportManager> _logger;
        [BindProperty]
        [Required(ErrorMessage = "Soubor je povinn�")]
        [DisplayName("Vlo�it excel...")]
        public IFormFile ExcelUpload { get; set; }

        public IList<TimesheetImport> TimesheetImport { get; set; }
        [BindProperty]
        public string TimesheetImportJSON { get; set; }

        [BindProperty]
        [DisplayName("Obej�t propustn� chyby")]
        public bool OverrideErrors { get; set; }


        public TimesheetModel(IImportManager importManager, ILogger<IImportManager> logger)
        {
            _importManager = importManager;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            byte[] source = null;
            if (!(ExcelUpload is null) && ExcelUpload.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ExcelUpload.CopyTo(ms);
                    source = ms.ToArray();
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                TimesheetImport = _importManager.ConvertPeople(source);
                ModelState.AddModelError("Success", string.Format("Bylo na�teno {0} z�znam�.", TimesheetImport.Count()));
                if (TimesheetImport.Any(x => !x.Success))
                    ModelState.AddModelError("Error", string.Format("P�i na��t�n� bylo nalezeno {0} chybn�ch z�znam�.", TimesheetImport.Where(x => !x.Success).Count()));
                TimesheetImportJSON = JsonConvert.SerializeObject(TimesheetImport, Formatting.None, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "P�i importu nastala chyba");
                return await this.PageWithError();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostSaveAsync()
        {
            TimesheetImport = JsonConvert.DeserializeObject<IList<TimesheetImport>>(TimesheetImportJSON);
            try
            {
                await _importManager.Import(TimesheetImport, OverrideErrors);
                ModelState.AddModelError("Success", string.Format("Bylo ulo�eno {0} z�znam�.", TimesheetImport.Count()));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "P�i importu nastala chyba");
                return await this.PageWithError("P�i na��t�n� dat nastala chyba.");
            }
            TimesheetImport = null;
            return Page();
        }
    }
}
