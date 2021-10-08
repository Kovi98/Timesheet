using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portal.Areas.Identity;
using Portal.Data;
using System;
using Timesheet.DocManager.Entities;
using Timesheet.DocManager.Services;
using Timesheet.Entity.Entities;
using Timesheet.Entity.Services;

namespace Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //CultureInfo - cs-CZ
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("cs-CZ");
            });
            //Dependency injection
            #region DI
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("Identity")));
            services.AddDbContext<TimesheetContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("Timesheet")));
            services.AddDbContext<DocumentContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("Document")));
            #endregion
            //Custom configs
            services.Configure<PaymentOptions>(Configuration.GetSection(PaymentOptions.CONFIG_SECTION_NAME));
            //Custom services
            services.AddScoped<IDocumentManager, DocumentManager>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITimesheetService, TimesheetService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            //Konfigurace IDENTITY - TODO dodìlat
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                //Potvrzení účtu
                options.SignIn.RequireConfirmedEmail = false;
            });

            // Cookie settings
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddAuthorization(options =>
                {
                    options.AddPolicy("VerifiedPolicy", policy =>
                        policy.RequireRole("Member"));
                    options.AddPolicy("AdminPolicy", policy =>
                        policy.RequireRole("Admin"));
                });

            services.AddRazorPages().AddDataAnnotationsLocalization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Autentizace a autorizace
            app.UseAuthentication();
            app.UseAuthorization();

            //Localization - CultureInfo
            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages().RequireAuthorization("VerifiedPolicy");
            });

            app.UseStatusCodePagesWithRedirects("/Errors/{0}");
        }
    }
}