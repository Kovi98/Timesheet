using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business.Extensions
{
    public static class RegisterDI
    {
        public static IServiceCollection RegisterConfigs(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<PaymentOptions>(config.GetSection(PaymentOptions.CONFIG_SECTION_NAME));
            return services;
        }
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IDocumentManager, DocumentManager>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITimesheetService, TimesheetService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<IFinanceService, FinanceService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IRewardSummaryService, RewardSummaryService>();
            return services;
        }
        public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<TimesheetContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("Timesheet")));
            return services;
        }
    }
}
