using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Entity.Entities;
using Timesheet.Entity.Interfaces;

namespace Portal.Areas.Payments.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly ITimesheetService _timesheetService;
        private readonly ILogger _logger;

        public IndexModel(IPaymentService paymentService, ILogger logger, ITimesheetService timesheetService)
        {
            _paymentService = paymentService;
            _timesheetService = timesheetService;
            _logger = logger;
        }

        public IList<Payment> Payment { get; set; }
        [BindProperty]
        public Payment PaymentDetail { get; set; }
        public bool IsEditable { get; set; }
        public SelectList Timesheets { get; set; }
        [BindProperty]
        public int[] TimesheetsSelected { get; set; }


        public async Task OnGetAsync(int id)
        {
            await LoadData();
            var payment = Payment.FirstOrDefault(t => t.Id == id);
            if (id > 0 && payment != null)
            {
                PaymentDetail = payment;
            }
            IsEditable = false;
        }

        public async Task OnGetNotPaidAsync()
        {
            await LoadData();
            Payment = Payment.Where(p => !p.IsPaid).ToList();
            IsEditable = false;
        }

        public async Task OnGetEditAsync(int id)
        {
            await LoadData();
            var payment = await _paymentService.GetAsync(id);
            if (id > 0)
            {
                PaymentDetail = payment;
                TimesheetsSelected = payment.Timesheet.Select(x => x.Id).ToArray();
            }
            else
            {
                PaymentDetail = null;
            }
            IsEditable = true;
            var freeTimesheets = await _timesheetService.GetFreesAsync();
            Timesheets = new SelectList(freeTimesheets, "Id", "FriendlyName");
        }

        public async Task OnGetCreateFromTimesheetAsync(int[] ids)
        {
            await LoadData();
            var freeTimesheets = await _timesheetService.GetFreesAsync();
            Timesheets = new SelectList(freeTimesheets, "Id", "FriendlyName");
            PaymentDetail = null;
            TimesheetsSelected = freeTimesheets.Where(x => ids.Any(y => y == x.Id)).Select(x => x.Id).ToArray();
            IsEditable = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || PaymentDetail.IsPaid)
            {
                return RedirectToPage("./Index", new { id = PaymentDetail?.Id, area = "Payments" });
            }
            PaymentDetail.Timesheet = (await _timesheetService.GetAsync(false)).Where(x => TimesheetsSelected.Any(y => y == x.Id)).ToList();

            try
            {
                if (PaymentDetail.Id > 0)
                {
                    var oldTimesheets = (await _paymentService.GetAsync(PaymentDetail.Id))?.Timesheet;
                    var timesheetsToRemove = new List<Timesheet.Entity.Entities.Timesheet>();
                    foreach (var timesheet in oldTimesheets)
                    {
                        if (!PaymentDetail.Timesheet.Any(x => x.Id == timesheet.Id))
                        {
                            timesheetsToRemove.Add(timesheet);
                        }
                    }
                    foreach (var timesheet in timesheetsToRemove)
                    {
                        timesheet.Payment = null;
                        _timesheetService.SetModified(timesheet);
                    }
                    foreach (var timesheet in PaymentDetail.Timesheet)
                    {
                        timesheet.PaymentId = PaymentDetail.Id;
                        _timesheetService.SetModified(timesheet);
                    }
                }
                else
                {
                    PaymentDetail.CreateTime = DateTime.Now;
                    await _paymentService.SaveAsync(PaymentDetail);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _paymentService.ExistsAsync(PaymentDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Index", new { id = PaymentDetail.Id, area = "Payments" });
        }

        public async Task<IActionResult> OnPostPayAsync(int id)
        {
            if (!await _paymentService.ExistsAsync(id))
            {
                return NotFound();
            }

            var paymentToPay = await _paymentService.GetAsync(id);

            if (paymentToPay != null)
            {
                try
                {
                    if (_paymentService.TryPay(paymentToPay))
                        await _paymentService.SaveAsync(paymentToPay);
                    else
                        return BadRequest();
                }
                catch
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToPage("Index", new { id = PaymentDetail.Id, area = "Payments" });
        }

        public async Task<IActionResult> OnPostDownloadPayment(int id, string returnUrl = null)
        {
            var payment = await _paymentService.GetAsync(id);
            if (payment is null)
            {
                return BadRequest();
            }

            return File(Encoding.UTF8.GetBytes(payment.PaymentXml), "application/xml", payment.PaymentDateTime.Value.ToString("ddMMyyyy") + ".xml");
        }

        /// <summary>
        /// Smazání objektu Payment
        /// </summary>
        /// <param name="id">Id objektu</param>
        /// <returns>404 - NotFound / OkResult</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!await _paymentService.ExistsAsync(id))
            {
                return NotFound();
            }
            var paymentToDelete = await _paymentService.GetAsync(id);

            if (paymentToDelete.IsPaid)
            {
                return BadRequest();
            }

            if (paymentToDelete != null)
            {
                await _paymentService.RemoveAsync(paymentToDelete);
            }
            else
            {
                return NotFound();
            }

            return new OkResult();
        }
        private async Task LoadData()
        {
            Payment = await _paymentService.GetAsync();
        }
    }
}
