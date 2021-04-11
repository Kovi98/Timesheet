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
        byte[] GetContract(Person person, DocumentStorage defaultDocument);
    }
}
