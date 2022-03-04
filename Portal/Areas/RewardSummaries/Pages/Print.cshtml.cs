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
            if (personId == 0)
            {
                Lines = Lines
                    .GroupBy(x => x.PersonId)
                    .Select(x =>
                    {
                        return new RewardSummary()
                        {
                            Hours = x.Select(x => x.Hours).Sum(),
                            Reward = x.Select(x => x.Reward).Sum(),
                            Tax = x.Select(x => x.Tax).Sum(),
                            PersonId = x.Key,
                            Person = _personService.GetAsync(x.Key).GetAwaiter().GetResult()
                        };
                    })
                    .ToList();
            }
            RewardSummaryDetail = new RewardSummary
            {
                Hours = Lines.Select(x => x.Hours).Sum(),
                Reward = Lines.Select(x => x.Reward).Sum(),
                Tax = Lines.Select(x => x.Tax).Sum(),
                CreateDateTimeYear = year,
                CreateDateTimeMonth = month,
                PersonId = personId,
                Person = await _personService.GetAsync(personId)
            };
        }
    }
}
