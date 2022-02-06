﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class PaymentService : EntityServiceBase<Payment>
    {
        private readonly PaymentOptions _paymentOptions;
        public PaymentService(IOptions<PaymentOptions> paymentOptions, TimesheetContext context) : base(context)
        {
            _paymentOptions = paymentOptions.Value;
        }

        public override async Task<Payment> GetAsync(int id)
        {
            return await _context.Payment
                .Include(x => x.Timesheet)
                    .ThenInclude(x => x.Person)
                    .ThenInclude(x => x.PaidFrom)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<List<Payment>> GetAsync(bool asNoTracking = true)
        {
            var payments = _context
                .Payment
                .Include(x => x.Timesheet)
                .ThenInclude(x => x.Person)
                .ThenInclude(x => x.PaidFrom);
            return await (asNoTracking ? payments.AsNoTracking().ToListAsync() : payments.ToListAsync());
        }

        public bool TryPay(Payment payment)
        {
            return payment.TryCreatePaymentOrder(_paymentOptions.BankAccount);
        }

        public decimal GetPayedAmountInCurrentMonth()
        {
            return _context.Payment
                .Include(x => x.Timesheet)
                    .ThenInclude(x => x.Person)
                    .ThenInclude(x => x.PaidFrom)
                    .Where(p => p.PaymentDateTime != null && p.PaymentDateTime.Value.Year == DateTime.Now.Year && p.PaymentDateTime.Value.Month == DateTime.Now.Month).Select(p => p.RewardToPay).ToList().Sum();
        }
    }

    public class PaymentOptions
    {
        public const string CONFIG_SECTION_NAME = "Payments";
        public string BankAccount { get; set; }
        public decimal? Tax { get; set; }
    }
}
