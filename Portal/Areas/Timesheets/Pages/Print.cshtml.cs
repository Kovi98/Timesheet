using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Timesheet.Business;

namespace Portal.Pages.Timesheets
{
    public class PrintModel : PageModel
    {
        private readonly TimesheetService _timesheetService;

        public PrintModel(TimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }

        public Timesheet.Common.Timesheet Timesheet { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await _timesheetService.ExistsAsync(id))
            {
                return NotFound();
            }
            Timesheet = await _timesheetService.GetAsync(id);
            return Page();
        }
    }
}
