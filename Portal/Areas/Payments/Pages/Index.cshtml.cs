using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Business;
using Timesheet.Common;

namespace Portal.Areas.Payments.Pages
{
    public class IndexModel : PageModel, ILoadablePage
    {
        private readonly PaymentService _paymentService;
        private readonly TimesheetService _timesheetService;

        public IndexModel(PaymentService paymentService, TimesheetService timesheetService)
        {
            _paymentService = paymentService;
            _timesheetService = timesheetService;
        }

        public List<Payment> Payment { get; set; }
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
            var freeTimesheets = await _timesheetService.GetFreesAsync();
            if (id > 0)
            {
                PaymentDetail = payment;
                TimesheetsSelected = payment.PaymentItem.SelectMany(x => x.Timesheet).Select(x => x.Id).ToArray();
                freeTimesheets.AddRange(payment.PaymentItem.SelectMany(x => x.Timesheet));
            }
            else
            {
                PaymentDetail = null;
            }
            IsEditable = true;
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

            try
            {
                await _paymentService.GenerateItemsAsync(PaymentDetail, TimesheetsSelected);
                await _paymentService.SaveAsync(PaymentDetail);
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
            catch (Exception ex)
            {
                return await this.PageWithError();
            }

            return RedirectToPage("Index", new { id = PaymentDetail.Id, area = "Payments" });
        }

        public async Task<IActionResult> OnPostPayAsync(int id)
        {
            try
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
            }
            catch (Exception)
            {
                return await this.PageWithError();
            }
            return RedirectToPage("Index", new { id = PaymentDetail.Id, area = "Payments" });
        }

        public async Task<IActionResult> OnPostDownloadPayment(int id, string returnUrl = null)
        {
            var payment = await _paymentService.GetAsync(id);
            if (payment is null)
            {
                return await this.PageWithError();
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
                return await this.PageWithError();
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
        public async Task LoadData()
        {
            Payment = await _paymentService.GetAsync();
        }
    }
}
