using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Timesheet.DocManager.Entities;
using Timesheet.Entity.Entities;

namespace Timesheet.DocManager.Models
{
    public interface IDocumentManager
    {
        byte[] GetContract(Person person, DocumentStorage defaultDocument);
    }
}
