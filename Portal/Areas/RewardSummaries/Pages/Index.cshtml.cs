using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Common;

namespace Portal.Areas.RewardSummaries.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRewardSummaryService _rewardSummaryService;

        public IndexModel(IRewardSummaryService rewardSummaryService)
        {
            _rewardSummaryService = rewardSummaryService;
        }

        public IList<RewardSummary> RewardSummary { get; set; }

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            RewardSummary = await _rewardSummaryService.GetAsync();
        }
    }
}
