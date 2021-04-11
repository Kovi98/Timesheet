using System;
using System.Collections.Generic;
using System.Text;

namespace Timesheet.Entity.Entities
{
    public partial class Person
    {
        public string FullName => Name + " " + Surname;

        public string FullBankAccount => BankAccount + "/" + BankCode;

        public string FullAddress => Street + " " + HouseNumber + ", " + PostalCode + " " + City;
    }
}
