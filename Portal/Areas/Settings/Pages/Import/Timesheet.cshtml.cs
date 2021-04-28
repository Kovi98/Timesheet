using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Models;

namespace Portal.Areas.Settings.Pages.Import
{
    public class TimesheetModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;
        [BindProperty]
        [Required(ErrorMessage = "Soubor je povinný")]
        [DisplayName("Vložit excel...")]
        public IFormFile ExcelUpload { get; set; }

        [BindProperty]
        public IList<TimesheetImport> TimesheetImport { get; set; }

        public TimesheetModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
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
                var importManager = new ImportManager(_context);
                TimesheetImport = importManager.ConvertPeople(source);
                //await _context.Timesheet.AddRangeAsync(importData);
                //await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return Page();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostSaveAsync(bool overrideErrors = false)
        {
            try
            {
                if (!overrideErrors)
                    TimesheetImport = TimesheetImport.Where(x => x.ShouldImport).ToList();
                await _context.Timesheet.AddRangeAsync(TimesheetImport.Select(x => x.Timesheet));
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Page();
            }

            return Page();
        }
    }
}
