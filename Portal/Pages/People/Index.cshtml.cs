﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;
using Timesheet.Entity.Models;

namespace Portal.Pages.People
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;
        private readonly IDocumentManager _documentManager;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context, IDocumentManager documentManager)
        {
            _context = context;
            _documentManager = documentManager;
        }

        public IList<Person> Person { get;set; }
        [BindProperty]
        public Person PersonDetail { get; set; }
        public bool IsEditable { get; set; }

        public async Task OnGetAsync(int id)
        {
            Person = await _context.Person
                .Include(p => p.Job)
                .Include(p => p.PaidFrom)
                .Include(p => p.Section).ToListAsync();
            var person = Person.FirstOrDefault(t => t.Id == id);
            if (id > 0 && person != null)
            {
                PersonDetail = person;
            }
            ViewData["JobId"] = new SelectList(_context.Job, "Id", "Name");
            ViewData["PaidFromId"] = new SelectList(_context.Finance, "Id", "Name");
            ViewData["SectionId"] = new SelectList(_context.Section, "Id", "Name");
            IsEditable = false;
        }

        public async Task OnGetEditAsync(int id)
        {
            Person = await _context.Person
                .Include(p => p.Job)
                .Include(p => p.PaidFrom)
                .Include(p => p.Section).ToListAsync();
            var person = Person.FirstOrDefault(t => t.Id == id);
            if (id > 0)
            {
                PersonDetail = person;
            }
            else
            {
                PersonDetail = null;
            }
            ViewData["JobId"] = new SelectList(_context.Job, "Id", "Name");
            ViewData["PaidFromId"] = new SelectList(_context.Finance, "Id", "Name");
            ViewData["SectionId"] = new SelectList(_context.Section, "Id", "Name");
            IsEditable = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (PersonDetail.Id > 0)
            {
                _context.Attach(PersonDetail).State = EntityState.Modified;
            }
            else
            {
                PersonDetail.CreateTime = DateTime.Now;
                _context.Person.Add(PersonDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(PersonDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
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
            if (_context.Timesheet.Any(x => x.PersonId == id))
            {
                ModelState.AddModelError("Error", "Nelze smazat trenéra s existujícím výkazem!");
                return Page();
            }

            var timesheetToDelete = await _context.Person.FindAsync(id);

            if (Person != null)
            {
                _context.Person.Remove(timesheetToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }

        public async Task<FileContentResult> OnPostDownloadContract(int id)
        {
            return null;
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.Id == id);
        }
    }
}
