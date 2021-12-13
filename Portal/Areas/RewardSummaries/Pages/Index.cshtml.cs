using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Business;
using Timesheet.Common;

namespace Portal.Areas.RewardSummaries.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RewardSummaryService _rewardSummaryService;

        public IndexModel(RewardSummaryService rewardSummaryService)
        {
            _rewardSummaryService = rewardSummaryService;
        }

        public List<RewardSummary> RewardSummary { get; set; }

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
