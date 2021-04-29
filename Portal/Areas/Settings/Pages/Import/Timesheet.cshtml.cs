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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Portal.Models;
using Timesheet.Entity.Entities;

namespace Portal.Areas.Settings.Pages.Import
{
    public class TimesheetModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;
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
                ModelState.AddModelError("Success", string.Format("Bylo na�teno {0} z�znam�.", TimesheetImport.Count()));
                if (TimesheetImport.Any(x => !x.Success))
                    ModelState.AddModelError("Error", string.Format("P�i na��t�n� bylo nalezeno {0} chybn�ch z�znam�.", TimesheetImport.Where(x => !x.Success).Count()));
                TimesheetImportJSON = JsonConvert.SerializeObject(TimesheetImport);
            }
            catch(Exception e)
            {
                return Page();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostSaveAsync()
        {
            TimesheetImport = JsonConvert.DeserializeObject<IList<TimesheetImport>>(TimesheetImportJSON);
            var defaultList = TimesheetImport;
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (!OverrideErrors)
                {
                    TimesheetImport = TimesheetImport.Where(x => x.Success).ToList();
                }
                else
                {
                    TimesheetImport = TimesheetImport.Where(x => x.CanPassErrors).ToList();
                    var people = new List<Person>();
                    var jobs = new List<Job>();
                    //Hled�n� undefined jobs/people
                    foreach (var import in TimesheetImport)
                    {
                        if (import.Errors.Contains(TimesheetImportError.PersonUndefined) && !people.Contains(import.Timesheet.Person))
                        {
                            people.Add(import.Timesheet.Person);
                        }
                        if (import.Errors.Contains(TimesheetImportError.JobUndefined) && !jobs.Contains(import.Timesheet.Job))
                        {
                            jobs.Add(import.Timesheet.Job);
                        }
                    }

                    //Nahrazov�n� undefined people
                    foreach (var person in people)
                    {
                        _context.Person.Add(person);
                        foreach (var item in TimesheetImport.Where(x => x.Timesheet.Person.FullName == person.FullName).ToList())
                        {
                            item.Timesheet.Person = person;
                        }
                    }
                    //Nahrazov�n� undefined jobs
                    foreach (var job in jobs)
                    {
                        _context.Job.Add(job);
                        foreach (var item in TimesheetImport.Where(x => x.Timesheet.Job.Name == job.Name).ToList())
                        {
                            item.Timesheet.Job = job;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                //Samotn� ukl�d�n� timesheetu do DB
                var test = _context.ChangeTracker.Entries().ToList();
                //await _context.Timesheet.AddRangeAsync(TimesheetImport.Select(x => x.Timesheet));
                foreach(var timesheet in TimesheetImport.Select(x => x.Timesheet))
                {
                    _context.Timesheet.Add(timesheet);
                    _context.Entry(timesheet.Person).State = EntityState.Detached;
                    _context.Entry(timesheet.Job).State = EntityState.Detached;
                    await _context.SaveChangesAsync();
                }
                transaction.Commit();
                ModelState.AddModelError("Success", string.Format("Bylo ulo�eno {0} z�znam�.", TimesheetImport.Count()));
            }
            catch (Exception e)
            {
                transaction.Rollback();
                ModelState.AddModelError("Error", "Z�znamy se nepoda�ilo ulo�it.");
                TimesheetImport = defaultList;
                return Page();
            }
            TimesheetImport = null;
            return Page();
        }
    }
}
