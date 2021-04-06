using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Pages.People
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IList<Person> Person { get;set; }
        public Person PersonDetail { get; set; }
        public bool IsEditable { get; set; }

        public async Task OnGetAsync()
        {
            Person = await _context.Person
                .Include(p => p.Job)
                .Include(p => p.PaidFrom)
                .Include(p => p.Section).ToListAsync();
        }
    }
}
