using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class JobService : EntityServiceBase<Job>, IJobService
    {
        public JobService(TimesheetContext context) : base(context)
        {

        }
    }
}
