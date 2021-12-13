using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Business;
using Timesheet.Common;

namespace Portal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly TimesheetService _timesheetService;
        private readonly PersonService _personService;
        private readonly RewardSummaryService _rewardSummaryService;
        private readonly PaymentService _paymentService;

        public IndexModel(ILogger<IndexModel> logger, TimesheetService timesheetService, PersonService personService, RewardSummaryService rewardSummaryService, PaymentService paymentService)
        {
            _logger = logger;
            _timesheetService = timesheetService;
            _personService = personService;
            _rewardSummaryService = rewardSummaryService;
            _paymentService = paymentService;
        }

        [BindProperty]
        public List<Timesheet.Common.Timesheet> Timesheet { get; set; }
        [BindProperty]
        public List<Person> Person { get; set; }
        [BindProperty]
        public List<RewardSummary> RewardSummary { get; set; }
        [BindProperty]
        public List<Payment> Payment { get; set; }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        public async Task LoadData()
        {
            Timesheet = await _timesheetService.GetAsync();
            Person = await _personService.GetAsync();
            Payment = await _paymentService.GetAsync();
            RewardSummary = await _rewardSummaryService.GetAsync();
        }
    }
}
