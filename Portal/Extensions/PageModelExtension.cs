using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Portal.Extensions
{
    public static class PageModelExtension
    {
        public static async Task<PageResult> PageWithError(this PageModel pageModel, string error)
        {
            pageModel.ModelState.AddModelError("Error", error);
            if (pageModel is ILoadablePage castedPageModel)
                await castedPageModel.LoadData();
            return pageModel.Page();
        }

        public static async Task<PageResult> PageWithError(this PageModel pageModel)
        {
            pageModel.ModelState.AddModelError("Error", "Nastala chyba.");
            if (pageModel is ILoadablePage castedPageModel)
                await castedPageModel.LoadData();
            return pageModel.Page();
        }

    }

    public interface ILoadablePage
    {
        Task LoadData();
    }
}
