using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Business;
using Timesheet.Common;

namespace Portal.Areas.RewardSummaries.Pages
{
    public class PrintModel : PageModel
    {
        private readonly RewardSummaryService _rewardSummaryService;
        private readonly PersonService _personService;

        public PrintModel(RewardSummaryService rewardSummaryService, PersonService personService)
        {
            _rewardSummaryService = rewardSummaryService;
            _personService = personService;
        }

        public List<RewardSummary> Lines { get; set; }
        public RewardSummary RewardSummaryDetail { get; set; }

        public async Task OnGetAsync(int year, int month, int personId)
        {
            Lines = await _rewardSummaryService.GetAsync(year, month, personId);

            RewardSummaryDetail = new RewardSummary
            {
                Hours = Lines.Select(x => x.Hours).Sum(),
                Reward = Lines.Select(x => x.Reward).Sum(),
                Tax = Lines.Select(x => x.Tax).Sum(),
                RewardToPay = Lines.Select(x => x.RewardToPay).Sum(),
                Year = year,
                Month = month,
                PersonId = personId,
                Person = personId == 0 ? null : await _personService.GetAsync(personId)
            };
        }
    }
}
