using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Person
    {
        public Person()
        {
            Timesheet = new HashSet<Timesheet>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateBirth { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime CreateTime { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public decimal? HourReward { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public bool IsActive { get; set; }
        public bool HasTax { get; set; }
        public int SectionId { get; set; }
        public int PayedFromId { get; set; }
        public int JobId { get; set; }
        public string IdentityDocument { get; set; }

        public virtual Job Job { get; set; }
        public virtual Finance PayedFrom { get; set; }
        public virtual Section Section { get; set; }
        public virtual ICollection<Timesheet> Timesheet { get; set; }
    }
}
