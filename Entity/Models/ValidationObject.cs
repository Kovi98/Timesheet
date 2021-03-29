using System;
using System.Collections.Generic;
using System.Text;

namespace Timesheet.Entity.Models
{
    public class ValidationObject
    {
        public bool IsOk { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
}
