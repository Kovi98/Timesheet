using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;

namespace Portal.Areas.People.Pages
{
    public class IndexModel : PageModel, ILoadablePage
    {
        private readonly IPersonService _personService;
        private readonly IDocumentManager _documentManager;

        private readonly IJobService _jobService;
        private readonly IFinanceService _financeService;
        private readonly ISectionService _sectionService;

        public IndexModel(IPersonService personService, IDocumentManager docManager, IJobService jobService, IFinanceService financeService, ISectionService sectionService)
        {
            _personService = personService;
            _documentManager = docManager;
            _jobService = jobService;
            _financeService = financeService;
            _sectionService = sectionService;
        }

        public List<Person> Person { get; set; }

        [BindProperty]
        public Person PersonDetail { get; set; }

        public bool IsEditable { get; set; }

        [BindProperty]
        public string Format { get; set; }

        public async Task OnGetAsync(int id)
        {
            await LoadData();
            PersonDetail = await _personService.GetAsync(id);
            IsEditable = false;
        }

        public async Task OnGetEditAsync(int id)
        {
            await LoadData();
            PersonDetail = await _personService.GetAsync(id);
            IsEditable = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            try
            {
                await _personService.SaveAsync(PersonDetail);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _personService.ExistsAsync(PersonDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                return await this.PageWithError();
            }

            return RedirectToPage("./Index", new { id = PersonDetail.Id });
        }

        /// <summary>
        /// Smazání objektu Person
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var personToDelete = await _personService.GetAsync(id);

            if (personToDelete.Timesheet.Any())
            {
                return await this.PageWithError("Nelze smazat trenéra s existujícím výkazem.");
            }

            if (personToDelete != null)
            {
                await _personService.RemoveAsync(personToDelete);
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        public async Task<IActionResult> OnPostDownloadContract(int id)
        {
            try
            {
                string format = Format ?? "Docx";
                var defaultDocument = await _documentManager.GetDefaultDocumentAsync();

                if (defaultDocument == null)
                {
                    ModelState.AddModelError("Error", "Neexistuje žádná primární šablona smlouvy!");
                    return Page();
                }
                var person = await _personService.GetAsync(id);

                if (person is null || defaultDocument is null)
                    return NotFound();
                var document = await _documentManager.GenerateContract(person, defaultDocument);
                return File(document, _documentManager.GetContentType(format), $"export.{format}");
            }
            catch (Exception)
            {
                return await this.PageWithError();
            }
        }

        public async Task LoadData()
        {
            Person = await _personService.GetAsync();
            ViewData["JobId"] = new SelectList(await _jobService.GetAsync(), "Id", "Name");
            ViewData["PaidFromId"] = new SelectList(await _financeService.GetAsync(), "Id", "Name");
            ViewData["SectionId"] = new SelectList(await _sectionService.GetAsync(), "Id", "Name");
        }
    }
}
