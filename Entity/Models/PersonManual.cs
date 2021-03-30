using System;
using System.Collections.Generic;
using System.Text;

namespace Timesheet.Entity.Entities
{
    public partial class Person
    {
        public string FullName { get { return Name + " " + Surname; } }
    }
}
