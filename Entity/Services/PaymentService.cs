using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.Entity.Entities;
using Timesheet.Entity.Interfaces;

namespace Timesheet.Entity.Services
{
    public class PaymentService : EntityServiceBase<Payment>, IPaymentService
    {
        private readonly PaymentOptions _paymentOptions;
        private readonly TimesheetContext _context;
        public PaymentService(IOptions<PaymentOptions> paymentOptions, TimesheetContext context) : base(context)
        {
            _paymentOptions = paymentOptions.Value;
            _context = context;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            return await _context.Payment.AnyAsync(x => x.Id == id);
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
    }

    public class PaymentOptions
    {
        public const string CONFIG_SECTION_NAME = "Payments";
        public string BankAccount { get; set; }
    }
}
