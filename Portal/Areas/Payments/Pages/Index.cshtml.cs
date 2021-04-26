﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Timesheet.Entity.Entities;

namespace Portal.Areas.Payments.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Timesheet.Entity.Entities.TimesheetContext _context;

        public IndexModel(Timesheet.Entity.Entities.TimesheetContext context)
        {
            _context = context;
        }

        public IList<Payment> Payment { get;set; }
        [BindProperty]
        public Payment PaymentDetail { get; set; }
        public bool IsEditable { get; set; }
        public SelectList Timesheets { get; set; }
        [BindProperty]
        public int[] TimesheetsSelected { get; set; }


        public async Task OnGetAsync(int id)
        {
            Payment = await _context.Payment
                .Include(p => p.Timesheet)
                .ToListAsync();
            var payment = Payment.FirstOrDefault(t => t.Id == id);
            if (id > 0 && payment != null)
            {
                PaymentDetail = payment;
            }
            IsEditable = false;
        }

        public async Task OnGetNotPaidAsync()
        {
            Payment = await _context.Payment
                .Include(p => p.Timesheet)
                .ToListAsync();
            Payment = Payment.Where(p => !p.IsPaid).ToList();
            IsEditable = false;
        }

        public async Task OnGetEditAsync(int id)
        {
            Payment = await _context.Payment
                .Include(p => p.Timesheet)
                .ToListAsync();
            var payment = Payment.FirstOrDefault(t => t.Id == id);
            if (id > 0)
            {
                PaymentDetail = payment;
            }
            else
            {
                PaymentDetail = null;
            }
            IsEditable = true;
            var freeTimesheets = _context.Timesheet.Include(x => x.Person).Where(x => x.PaymentId == 0 || x.PaymentId == null);
            Timesheets = new SelectList(freeTimesheets, "Id", "FriendlyName");
            TimesheetsSelected = await _context.Timesheet.Where(x => x.PaymentId == id && x.PaymentId != null).Select(x => x.Id).ToArrayAsync();
        }

        public async Task OnGetCreateFromTimesheetAsync(int[] ids)
        {
            Payment = await _context.Payment
                .Include(p => p.Timesheet)
                .ToListAsync();
            ViewData["Timesheets"] = new SelectList(_context.Timesheet.Include(x => x.Person).Where(x => x.PaymentId == 0), "Id", "FriendlyName");
            PaymentDetail = null;
            TimesheetsSelected = await _context.Timesheet.Include(x => x.Payment).Where(x => ids.Any(y => y == x.Id)).Select(x => x.Id).ToArrayAsync();
            IsEditable = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Index", new { id = PaymentDetail?.Id, area = "Payments" });
            }
            PaymentDetail.Timesheet = await _context.Timesheet.Where(x => (x.PaymentId == null || x.PaymentId == 0) && (TimesheetsSelected.Any(y => y == x.Id))).ToListAsync();
            if (PaymentDetail.Id > 0)
            {
                _context.Attach(PaymentDetail).State = EntityState.Modified;
            }
            else
            {
                PaymentDetail.CreateTime = DateTime.Now;
                _context.Payment.Add(PaymentDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(PaymentDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { id = PaymentDetail.Id, area = "Payments" });
        }

        /// <summary>
        /// Smazání objektu Payment
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var paymentToDelete = await _context.Payment.FindAsync(id);

            if (false)
            {
                //Existujicí platba
            }

            if (paymentToDelete != null)
            {
                _context.Payment.Remove(paymentToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }
        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.Id == id);
        }
    }
}
