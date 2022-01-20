using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Portal.Pages.Errors
{
    [AllowAnonymous]
    public class _500Model : PageModel
    {
        public void OnGet()
        {
        }
    }
}
