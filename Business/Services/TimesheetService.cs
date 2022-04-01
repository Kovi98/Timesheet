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
        protected override IQueryable<Common.Timesheet> DefaultQuery => _context.Timesheet
                                                                            .Include(t => t.Job)
                                                                            .Include(t => t.PaymentItem)
                                                                                .ThenInclude(p => p.Payment)
                                                                            .Include(t => t.Person)
                                                                            .Include(t => t.Person.Section)
                                                                            .Include(t => t.Person.PaidFrom);
        private readonly PaymentOptions _options;
        public TimesheetService(IOptions<PaymentOptions> options, TimesheetContext context) : base(context)
        {
            _options = options.Value;
        }

        public override async Task<Common.Timesheet> GetAsync(int id)
        {
            return await DefaultQuery
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Common.Timesheet>> GetMultipleAsync(IEnumerable<int> ids)
        {
            return await DefaultQuery
                .Where(x => ids.Any(y => y == x.Id))
                .ToListAsync();
        }

        public override async Task<List<Common.Timesheet>> GetAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await DefaultQuery
                    .AsNoTracking().ToListAsync()
                : await DefaultQuery
                    .ToListAsync();
        }

        public async Task<List<Common.Timesheet>> GetFreesAsync(bool asNoTracking = true)
        {
            return asNoTracking
                ? await DefaultQuery
                    .Where(x => x.PaymentItemId == 0 || x.PaymentItemId == null)
                    .AsNoTracking().ToListAsync()
                : await DefaultQuery
                    .Where(x => (x.PaymentItemId == 0 || x.PaymentItemId == null))
                    .ToListAsync();
        }

        public async Task<List<int>> GetPaymentTimesheetIdsAsync(int paymentId)
        {
            return await DefaultQuery.Where(x => x.PaymentItem.PaymentId == paymentId)
                    .Select(x => x.Id)
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
            return DefaultQuery
                .Where(t => t.PaymentItem == null || t.PaymentItem.Payment.PaymentDateTime == null).Count();
        }
        public decimal GetHoursThisMonth()
        {
            return DefaultQuery
                    .Where(t => t.DateTimeTo.HasValue && t.DateTimeTo.Value.Year == DateTime.Now.Year && t.DateTimeTo.Value.Month == DateTime.Now.Month && t.Hours.HasValue).Select(t => t.Hours.Value).Sum();
        }
        public List<Timesheet.Common.Timesheet> GetLastFive()
        {
            return DefaultQuery
                    .AsNoTracking()
                    .Where(t => t.DateTimeTo.HasValue).OrderByDescending(t => t.DateTimeTo).Take(5).ToList();
        }
        public void CalculateReward(Common.Timesheet timesheet)
        {
            if (timesheet == null) return;

            var hourReward = timesheet.Job?.HourReward ?? _context.Job.Where(x => x.Id == timesheet.JobId).Select(x => x.HourReward).FirstOrDefault() ?? 0;

            if (!timesheet.Hours.HasValue && timesheet.DateTimeFrom.HasValue && timesheet.DateTimeTo.HasValue)
                timesheet.Hours = (decimal)(timesheet.DateTimeTo - timesheet.DateTimeFrom)?.TotalHours;
            if (!timesheet.Reward.HasValue)
                timesheet.Reward = timesheet.Hours * hourReward;
        }
    }
}
