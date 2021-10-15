using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Data
{
    public class Initializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            //context.Database.EnsureCreated();
            // Look for any migrations
            if (context.Database.GetPendingMigrations().Any())
                await context.Database.MigrateAsync();

            context.SaveChanges();
        }
    }
}
