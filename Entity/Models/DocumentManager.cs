using System;
using System.Collections.Generic;
using System.Text;
using Timesheet.Entity.Entities;
using GemBox.Document;
using System.IO;
using System.Threading.Tasks;

namespace Timesheet.Entity.Models
{
    public class DocumentManager : IDocumentManager
    {
        private string _path;

        public DocumentManager(string path)
        {
            _path = path;
        }
        public MemoryStream GetContract(Person person)
        {
            return null;
        }
    }
}
