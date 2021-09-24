using Timesheet.Entity.Entities;

namespace Timesheet.Entity.Services
{
    public interface IPaymentService
    {
        public bool TryPay(Payment paymentToPay);
    }
}
