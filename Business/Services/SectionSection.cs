using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class SectionService : EntityServiceBase<Section>, ISectionService
    {
        public SectionService(TimesheetContext context) : base(context)
        {

        }
    }
}
