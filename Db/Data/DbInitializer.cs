using System;
using System.Linq;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Entity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TimesheetContext context)
        {
            context.Database.EnsureCreated();

            // Look for any jobs.
            if (context.Job.Any())
            {
                return;   // DB has been seeded
            }

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
            context.SaveChanges();
        }
    }
}
