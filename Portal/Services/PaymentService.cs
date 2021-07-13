using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Entity.Entities;

namespace Portal.Services
{
    public class PaymentService
    {
        private readonly PaymentOptions _paymentOptions;
        public PaymentService(IOptions<PaymentOptions> paymentOptions)
        {
            _paymentOptions = paymentOptions.Value;
        }

        public bool TryPay(Payment payment)
        {
            return payment.TryCreatePaymentOrder(_paymentOptions.BankAccount);
        }
    }

    public class PaymentOptions
    {
        public const string CONFIG_SECTION_NAME = "Payments";
        public string BankAccount { get; set; }
    }
}
