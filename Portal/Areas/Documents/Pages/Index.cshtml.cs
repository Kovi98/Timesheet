using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Timesheet.DocManager.Entities;

namespace Portal.Areas.Documents.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.DocManager.Entities.DocumentContext _context;

        public IndexModel(Timesheet.DocManager.Entities.DocumentContext context)
        {
            _context = context;
        }

        public IList<DocumentStorage> DocumentStorage { get;set; }

        public async Task OnGetAsync()
        {
            DocumentStorage = await _context.DocumentStorage.ToListAsync();
        }
    }
}
