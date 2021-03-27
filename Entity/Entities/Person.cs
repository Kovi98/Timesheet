using System;
using System.Collections.Generic;

#nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Person
    {
        public Person(string name, string surname, DateTime dateBirth, bool hasTax)
        {
            CreateTime = DateTime.Now;
            IsActive = true;

            Name = name;
            Surname = surname;
            DateBirth = dateBirth;
            HasTax = hasTax;

            //Vygenerováno EF
            Timesheets = new HashSet<Timesheet>();
        }
        #region EF DB first
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
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
        public virtual ICollection<Timesheet> Timesheets { get; set; }
        #endregion


    }
}
