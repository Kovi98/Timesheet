﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Timesheet.Entity.Entities;

namespace Portal.Pages.People
{
    public class CreateModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public CreateModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["JobId"] = new SelectList(_context.Job, "Id", "Name");
        ViewData["PaidFromId"] = new SelectList(_context.Finance, "Id", "Name");
        ViewData["SectionId"] = new SelectList(_context.Section, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Person Person { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Person.Add(Person);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
