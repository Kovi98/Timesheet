using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Entity.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(TimesheetContext context)
        {
            //context.Database.EnsureCreated();
            // Look for any migrations
            if (context.Database.GetPendingMigrations().Any())
                await context.Database.MigrateAsync();

            // Look for any jobs.
            if (!context.Job.Any())
            {
                var jobs = new Job[]
                {
                new Job{Name = "hlavní trenér", HourReward = (decimal)130, CreateTime = DateTime.Now},
                new Job{Name = "kondiční trenér", HourReward = (decimal)130, CreateTime = DateTime.Now},
                new Job{Name = "trenér brankářů", HourReward = (decimal)80, CreateTime = DateTime.Now},
                new Job{Name = "asistent trenéra", HourReward = (decimal)80, CreateTime = DateTime.Now},
                new Job{Name = "supertrenér", HourReward = (decimal)250, CreateTime = DateTime.Now},
                new Job{Name = "vedoucí družstva", HourReward = (decimal)250, CreateTime = DateTime.Now},
                new Job{Name = "redaktor", HourReward = null, CreateTime = DateTime.Now},
                new Job{Name = "grafik", HourReward = null, CreateTime = DateTime.Now}
                };
                context.AddRange(jobs);
            }

            // Look for any finances
            if (!context.Finance.Any())
            {
                var finances = new Finance[]
                {
                new Finance{Name = "MŠMT", CreateTime = DateTime.Now},
                new Finance{Name = "Provoz", CreateTime = DateTime.Now},
                new Finance{Name = "Vedení", CreateTime = DateTime.Now},
                new Finance{Name = "PP3", CreateTime = DateTime.Now}
                };
                context.Finance.AddRange(finances);
            }

            // Look for any finances
            if (!context.Section.Any())
            {
                var sections = new Section[]
                {
                new Section{Name = "mužská", CreateTime = DateTime.Now},
                new Section{Name = "ženská", CreateTime = DateTime.Now},
                new Section{Name = "vedoucí týmů", CreateTime = DateTime.Now}
                };
                context.Section.AddRange(sections);
            }

            context.SaveChanges();
        }

        public static async Task InitializeContractAsync(TimesheetContext context, IConfiguration config)
        {
            //context.Database.EnsureCreated();
            // Look for any migrations
            if (context.Database.GetPendingMigrations().Any())
                await context.Database.MigrateAsync();

            var path = config.GetValue<string>("Contracts:DefaultTemplatePath");
            if (!File.Exists(path))
                return;
            var file = new FileInfo(path);
            using var fileStream = file.OpenRead();
            using var ms = new MemoryStream();
            fileStream.CopyTo(ms);

            if (!context.DocumentStorage.Any())
            {
                var contract = new DocumentStorage()
                {
                    IsDefault = true,
                    DocumentName = file.Name,
                    Name = "Primární pracovní smlouva",
                    DocumentSource = ms.ToArray(),
                    CreateTime = DateTime.Now
                };
                context.Add(contract);
            }

            context.SaveChanges();
        }
    }
}
