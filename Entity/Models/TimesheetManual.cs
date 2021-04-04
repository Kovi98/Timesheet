using System;
using System.Collections.Generic;
using System.Text;

namespace Timesheet.Entity.Entities
{
    public partial class Timesheet
    {
        public decimal ToPay { get { return (Reward ?? 0) - Tax; } }
        public TimeSpan HoursTime
        {
            get
            {
                return TimeSpan.FromHours(Hours.HasValue ? (double)Hours.Value : 0);
            }
        }
    }
}
