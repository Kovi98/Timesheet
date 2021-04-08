using System;
using System.Collections.Generic;
using System.Text;
using Timesheet.Entity.Entities;
using GemBox.Document;

namespace Timesheet.Entity.Models
{
    public class DocumentManager : IDocumentManager
    {
        private string _path;

        public DocumentManager(string path)
        {
            _path = path;
        }
        public void GetContract(Person person)
        {

        }
        public void GetContract(IDictionary<string, string> personData)
        {

        }
    }
}
