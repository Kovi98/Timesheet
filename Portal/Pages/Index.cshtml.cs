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

        public int ActivePeopleCount { get; set; }
        public decimal PayedAmountInCurrentAmount { get; set; }
        public int TimesheetsWithoutPayedPayment { get; set; }
        public decimal TimesheetsHoursInCurrentMonth { get; set; }
        public List<Timesheet.Common.Timesheet> LastFiveTimesheets { get; set; }
        public List<Payment> LastFivePayments { get; set; }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        public async Task LoadData()
        {
            PayedAmountInCurrentAmount = _paymentService.GetPayedAmountInCurrentMonth();
            ActivePeopleCount = _personService.GetNumberOfActive();
            TimesheetsWithoutPayedPayment = _timesheetService.GetNumberOfNotPayed();
            TimesheetsHoursInCurrentMonth = _timesheetService.GetHoursThisMonth();
            LastFiveTimesheets = _timesheetService.GetLastFive();
            LastFivePayments = _paymentService.GetLastFive();
        }
    }
}
