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
                .Include(t => t.Payment)
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
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .AsNoTracking().ToListAsync()
                : await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
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
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => (x.PaymentId == 0 || x.PaymentId == null))
                    .AsNoTracking().ToListAsync()
                : await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => (x.PaymentId == 0 || x.PaymentId == null))
                    .AsNoTracking().ToListAsync();
        }

        public async Task<List<Common.Timesheet>> GetPaymentTimesheets(int paymentId, bool asNoTracking = true)
        {
            return asNoTracking
                ? await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => x.PaymentId == paymentId)
                    .AsNoTracking().ToListAsync()
                : await _context.Timesheet
                    .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(x => x.PaymentId == paymentId)
                    .AsNoTracking().ToListAsync();
        }

        public override Task SaveAsync(Common.Timesheet entity)
        {
            if (entity.DateTimeFrom > entity.DateTimeTo) throw new InvalidOperationException("Začátek výkazu nemůže být později než konec výkazu");
            entity.CalculateReward((_options.Tax / 100) ?? null, _context.Job.Find(entity.JobId)?.HourReward, _context.Person.Find(entity.PersonId)?.HasTax);
            return base.SaveAsync(entity);
        }

        public int GetNumberOfNotPayed()
        {
            return _context.Timesheet
                .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                .Where(t => t.Payment == null || t.Payment.PaymentDateTime == null).Count();
        }
        public decimal GetHoursThisMonth()
        {
            return _context.Timesheet
                .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(t => t.DateTimeTo.HasValue && t.DateTimeTo.Value.Year == DateTime.Now.Year && t.DateTimeTo.Value.Month == DateTime.Now.Month && t.Hours.HasValue).Select(t => t.Hours.Value).Sum();
        }
        public List<Timesheet.Common.Timesheet> GetLastFive()
        {
            return _context.Timesheet.AsNoTracking()
                .Include(t => t.Job)
                    .Include(t => t.Payment)
                    .Include(t => t.Person)
                    .Include(t => t.Person.Section)
                    .Include(t => t.Person.PaidFrom)
                    .Where(t => t.DateTimeTo.HasValue).OrderByDescending(t => t.DateTimeTo).Take(5).ToList();
        }
    }
}
