﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public Payment PaymentDetail { get; set; }
        public bool IsEditable { get; set; }
        public IList<Timesheet.Entity.Entities.Timesheet> Timesheet { get; set; }


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
            Timesheet = null;
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
        }

        public async Task OnGetCreateFromTimesheetAsync(int[] ids)
        {
            Payment = await _context.Payment
                .Include(p => p.Timesheet)
                .ToListAsync();
            PaymentDetail = null;
            Timesheet = await _context.Timesheet.Include(x => x.Payment).Where(x => ids.Any(y => y == x.Id)).ToListAsync();
            IsEditable = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Index", new { id = PaymentDetail?.Id, area = "Payments" });
            }

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
