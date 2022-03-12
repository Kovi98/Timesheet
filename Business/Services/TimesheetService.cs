using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class TimesheetService : EntityServiceBase<Common.Timesheet>
    {
        private readonly PaymentOptions _options;
        public TimesheetService(IOptions<PaymentOptions> options, TimesheetContext context) : base(context)
        {
            _options = options.Value;
        }

        public override async Task<Common.Timesheet> GetAsync(int id)
        {
            return await _context.Timesheet
                .Include(t => t.Job)
                .Include(t => t.PaymentItem)
                    .ThenInclude(p => p.Payment)
                .Include(t => t.Person)
                .Include(t => t.Person.Section)
                .Include(t => t.Person.PaidFrom)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public override async Task<List<Common.Timesheet>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .AsNoTracking().ToListAsync()
                : await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .ToListAsync();
        }

        public async Task<List<Common.Timesheet>> GetFreesAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => x.PaymentItemId == 0 || x.PaymentItemId == null)
                    .AsNoTracking().ToListAsync()
                : await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => (x.PaymentItemId == 0 || x.PaymentItemId == null))
                    .ToListAsync();
        }

        public async Task<List<Common.Timesheet>> GetPaymentTimesheetsAsync(int paymentId, bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => x.PaymentItem.PaymentId == paymentId)
                    .AsNoTracking().ToListAsync()
                : await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => x.PaymentItem.PaymentId == paymentId)
                    .ToListAsync();
        }

        public override Task SaveAsync(Common.Timesheet entity)
        {
            if (entity.DateTimeFrom > entity.DateTimeTo) throw new InvalidOperationException("Začátek výkazu nemůže být později než konec výkazu");
            CalculateReward(entity);
            return base.SaveAsync(entity);
        }

        public int GetNumberOfNotPayed()
        {
            return _context.Timesheet
                .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                .Where(t => t.PaymentItem == null || t.PaymentItem.Payment.PaymentDateTime == null).Count();
        }
        public decimal GetHoursThisMonth()
        {
            return _context.Timesheet
                .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(t => t.DateTimeTo.HasValue && t.DateTimeTo.Value.Year == DateTime.Now.Year && t.DateTimeTo.Value.Month == DateTime.Now.Month && t.Hours.HasValue).Select(t => t.Hours.Value).Sum();
        }
        public List<Timesheet.Common.Timesheet> GetLastFive()
        {
            return _context.Timesheet.AsNoTracking()
                .Include(t => t.Job)
                    .Include(t => t.PaymentItem)
                        .ThenInclude(p => p.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(t => t.DateTimeTo.HasValue).OrderByDescending(t => t.DateTimeTo).Take(5).ToList();
        }
        public void CalculateReward(Common.Timesheet timesheet)
        {
            if (timesheet == null) return;

            var hourReward = _context.Job.Where(x => x.Id == timesheet.JobId).Select(x => x.HourReward).FirstOrDefault() ?? 0;

            if (!timesheet.Hours.HasValue && timesheet.DateTimeFrom != null && timesheet.DateTimeTo != null)
                timesheet.Hours = (decimal)(timesheet.DateTimeTo - timesheet.DateTimeFrom)?.TotalHours;
            if (!timesheet.Reward.HasValue)
                timesheet.Reward = timesheet.Hours * hourReward;
        }
    }
}
