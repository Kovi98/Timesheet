using System;
using System.Collections.Generic;
using System.Text;

namespace Timesheet.Entity.Entities
{
    public partial class Payment
    {
        public bool IsPayed { get { return !(PaymentDateTime is null); } }
    }
}
