﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timesheet.Common
{
    public interface IPaymentService : IEntityService<Payment>
    {
        bool TryPay(Payment paymentToPay);
        Task SaveAsync(Payment payment);
        Task<Payment> GetAsync(int id);
        Task RemoveAsync(Payment payment);
        Task<List<Payment>> GetAsync(bool asNoTracking = true);
        Task<bool> ExistsAsync(int id);
    }
}
