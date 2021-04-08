using System;
using System.Collections.Generic;
using System.Text;
using Timesheet.Entity.Entities;

namespace Timesheet.Entity.Models
{
    public interface IDocumentManager
    {
        void GetContract(Person person);
        void GetContract(IDictionary<string, string> personData);
    }
}
