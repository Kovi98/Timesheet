using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
                foreach (Job j in jobs)
                {
                    context.Job.Add(j);
                }
            }

            //Check default document
            if (!context.DocumentStorage.Any(x => x.IsDefault))
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\default_template.docx");
                context.DocumentStorage.Add(new DocumentStorage()
                {
                    Name = "Základní šablona smlouvy",
                    IsDefault = true,
                    DocumentName = "default_template.docx",
                    DocumentSource = File.ReadAllBytes(path),
                    CreateTime = DateTime.Now
                });
            }

            context.SaveChanges();
        }
    }
}
