namespace Timesheet.Common
{
    public interface IPaymentService : IEntityService<Payment>
    {
        bool TryPay(Payment payment);
    }
}
