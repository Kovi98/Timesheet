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
    public class PersonModel : PageModel
    {
        private readonly IImportManager _importManager;
        private readonly ILogger<IImportManager> _logger;
        [BindProperty]
        [Required(ErrorMessage = "Soubor je povinný")]
        [DisplayName("Vložit csv...")]
        public IFormFile CsvUpload { get; set; }

        public List<PersonImport> PersonImport { get; set; }
        [BindProperty]
        public string PersonImportJSON { get; set; }

        [BindProperty]
        [DisplayName("Obejít propustné chyby")]
        public bool OverrideErrors { get; set; }


        public PersonModel(IImportManager importManager, ILogger<IImportManager> logger)
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
            if (!(CsvUpload is null) && CsvUpload.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    CsvUpload.CopyTo(ms);
                    source = ms.ToArray();
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                PersonImport = _importManager.ConvertPeople(source);
                ModelState.AddModelError("Success", string.Format("Bylo naèteno {0} záznamù.", PersonImport.Count()));
                if (PersonImport.Any(x => !x.Success))
                    ModelState.AddModelError("Error", string.Format("Pøi naèítání bylo nalezeno {0} chybných záznamù.", PersonImport.Where(x => !x.Success).Count()));
                PersonImportJSON = JsonConvert.SerializeObject(PersonImport, Formatting.None, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Pøi naèítání dat nastala chyba");
                return await this.PageWithError("Pøi naèítání dat nastala chyba");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostSaveAsync()
        {
            try
            {
                PersonImport = JsonConvert.DeserializeObject<List<PersonImport>>(PersonImportJSON);
                var count = await _importManager.Import(PersonImport, OverrideErrors);
                ModelState.AddModelError("Success", string.Format("Bylo uloženo {0} záznamù.", count));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Pøi importu nastala chyba");
                PersonImport = null;
                return await this.PageWithError("Pøi importu nastala chyba");
            }
            PersonImport = null;
            return Page();
        }
    }
}
