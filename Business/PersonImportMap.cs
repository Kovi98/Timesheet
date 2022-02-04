using CsvHelper.Configuration;
using Timesheet.Common.Models;

namespace Timesheet.Business
{
    public class PersonImportMap : ClassMap<PersonImportDto>
    {
        public PersonImportMap()
        {
            Map(x => x.Name).Optional();
            Map(x => x.Surname).Optional();
            Map(x => x.DateBirth).Optional();
            Map(x => x.HouseNumber).Optional();
            Map(x => x.Street).Optional();
            Map(x => x.PostalCode).Optional();
            Map(x => x.State).Optional();
            Map(x => x.BankAccount).Optional();
            Map(x => x.BankCode).Optional();
            Map(x => x.IsActive).Optional();
            Map(x => x.HasTax).Optional();
            Map(x => x.IdentityDocument).Optional();
            Map(x => x.Job).Optional();
            Map(x => x.PaidFrom).Optional();
            Map(x => x.Section).Optional();
            Map(x => x.City).Optional();
        }
    }
}
