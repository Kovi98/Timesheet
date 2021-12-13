using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Common.Interfaces;
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
            services.AddScoped<DocumentManager>();
            services.AddScoped<PaymentService>();
            services.AddScoped<TimesheetService>();
            services.AddScoped<JobService>();
            services.AddScoped<SectionService>();
            services.AddScoped<FinanceService>();
            services.AddScoped<PersonService>();
            services.AddScoped<RewardSummaryService>();
            services.AddScoped<IImportManager, ImportManager>();
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
