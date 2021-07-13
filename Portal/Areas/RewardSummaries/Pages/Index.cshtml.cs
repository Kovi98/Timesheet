using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;
using Microsoft.AspNetCore.Antiforgery;

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

        public async Task OnGetAsync()
        {
            RewardSummary = await _context.RewardSummary
            .Include(r => r.Person)
            .Include(r => r.Payment)
            .OrderByDescending(r => r.CreateDateTimeYear)
            .OrderByDescending(r => r.CreateDateTimeMonth).ToListAsync();
        }

        public async Task<IActionResult> OnGetHtmlPrintAsync(int year, int month, int personId)
        {
            try
            {
                string content = string.Empty;
                string url = Url.PageLink("/Print", null, new { year = year, month = month, personId = personId });
                    //_linkGenerator.GetUriByPage(this.HttpContext, "/Print", null, new { year = year, month = month, personId = personId });

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())  // Spusť request
                using (Stream responseStream = response.GetResponseStream())               // Načti response stream
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    content = await streamReader.ReadToEndAsync();
                }
                var result = new ContentResult();
                result.Content = content.Replace(Environment.NewLine, string.Empty).Replace(@"\", string.Empty);
                return result;
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
