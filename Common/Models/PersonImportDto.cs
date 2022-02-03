namespace Timesheet.Common.Models
{
    public class PersonImportDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DateBirth { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string IsActive { get; set; }
        public string HasTax { get; set; }
        public string IdentityDocument { get; set; }
        public string Job { get; set; }
        public string PaidFrom { get; set; }
        public string Section { get; set; }
    }
}
