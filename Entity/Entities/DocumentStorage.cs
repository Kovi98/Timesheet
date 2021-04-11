using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class DocumentStorage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] DocumentSource { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime CreateTime { get; set; }
        public string DocumentName { get; set; }
    }
}
