using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Areas.RewardSummaries.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IList<RewardSummary> RewardSummary { get;set; }
        public IList<Tuple<int?, IList<int?>>> YearsMonths { get; set; }

        public async Task OnGetAsync()
        {
            RewardSummary = await _context.RewardSummary
            .Include(r => r.Person)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth).ToListAsync();

            YearsMonths = new List<Tuple<int?, IList<int?>>>();
            foreach(var item in RewardSummary)
            {
                //Rok existuje
                var year = YearsMonths.Where(x => x.Item1 == item.CreateDateTimeYear);
                if (year.Count() > 0)
                {
                    //Měsíc neexistuje
                    if (YearsMonths.Where(x => x.Item1 == item.CreateDateTimeYear && x.Item2.Contains(item.CreateDateTimeMonth)).Count() == 0)
                    {
                        year.First().Item2.Add(item.CreateDateTimeMonth);
                    }
                }
                //Rok neexistuje
                else
                {
                    YearsMonths.Add(new Tuple<int?, IList<int?>>(item.CreateDateTimeYear, new List<int?>()));
                    YearsMonths.Where(x => x.Item1 == item.CreateDateTimeYear);
                }
            }
        }
    }
}
