using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Finance
    {
        public Finance(string name)
        {
            People = new HashSet<Person>();

            Name = name;
        }

        #region EF generated
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Person> People { get; set; }
        #endregion
    }
}
