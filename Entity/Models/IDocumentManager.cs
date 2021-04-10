using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Entity.Entities;

namespace Timesheet.Entity.Models
{
    public interface IDocumentManager
    {
        MemoryStream GetContract(Person person);
    }
}
