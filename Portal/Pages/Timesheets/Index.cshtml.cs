using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Pages.Timesheets
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IList<Timesheet.Entity.Entities.Timesheet> Timesheet { get;set; }
        [BindProperty]
        public Timesheet.Entity.Entities.Timesheet TimesheetDetail { get; set; }
        [BindProperty]
        public bool IsEditable { get; set; }
        public string TimesheetJson { get; set; }


        public async Task OnGetNotPayedAsync()
        {
            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person)
                .Where(t => t.Payment != null && !t.Payment.IsPayed).ToListAsync();
            TimesheetJson = Newtonsoft.Json.JsonConvert.SerializeObject(Timesheet.ToArray(), Newtonsoft.Json.Formatting.None,
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
            IsEditable = false;
        }

        public async Task OnGetAsync(int id)
        {
            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person).ToListAsync();
            var timesheet = Timesheet.FirstOrDefault(t => t.Id == id);
            if (id > 0 && timesheet != null)
            {
                TimesheetDetail = timesheet;
                IsEditable = false;
            }
        }

        public async Task OnGetEditAsync(int id)
        {
            Timesheet = await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.Payment)
                .Include(t => t.Person).ToListAsync();
            var timesheet = Timesheet.FirstOrDefault(t => t.Id == id);
            if (id > 0 && timesheet != null)
            {
                TimesheetDetail = timesheet;
                IsEditable = true;
            }
        }

        /// <summary>
        /// Smazání objektu Timesheet
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheetToDelete = await _context.Timesheet.FindAsync(id);

            if (Timesheet != null)
            {
                //_context.Timesheet.Remove(timesheetToDelete);
                //await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }
    }
}
