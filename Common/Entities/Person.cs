using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Common
{
    public partial class Person : IEntity
    {
        public Person()
        {
            Timesheet = new HashSet<Timesheet>();
        }

        public int Id { get; set; }

        [Display(Name = "Jméno")]
        [Required(ErrorMessage = "Jméno je povinné")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string Name { get; set; }

        [Display(Name = "Příjmení")]
        [Required(ErrorMessage = "Příjmení je povinné")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string Surname { get; set; }

        [Display(Name = "Datum narození")]
        [Required(ErrorMessage = "Datum narození je povinné")]
        [DataType(DataType.Date)]
        public DateTime DateBirth { get; set; }
        public byte[] RowVersion { get; set; }

        [Display(Name = "Vytvořeno", Description = "Datum a čas vytvoření záznamu")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "Ulice")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string Street { get; set; }

        [Display(Name = "č. p.")]
        [MaxLength(10, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string HouseNumber { get; set; }

        [Display(Name = "Město")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string City { get; set; }

        [Display(Name = "Stát")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string State { get; set; }

        [Display(Name = "PSČ")]
        [MaxLength(5, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string PostalCode { get; set; }

        [Display(Name = "Číslo účtu")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string BankAccount { get; set; }

        [Display(Name = "Kód banky")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string BankCode { get; set; }

        [Display(Name = "Aktivní", Description = "Je trenér aktivní?")]
        public bool IsActive { get; set; }

        [Display(Name = "Daň", Description = "Musí se odvádět srážková daň?")]
        public bool HasTax { get; set; }

        [Display(Name = "Sekce")]
        public int SectionId { get; set; }

        [Display(Name = "Dotace")]
        public int PaidFromId { get; set; }

        [Display(Name = "Pozice")]
        public int JobId { get; set; }

        [Display(Name = "Číslo osobního dokladu")]
        [MaxLength(50, ErrorMessage = "{0} může mít maximálně {1} znaků")]
        public string IdentityDocument { get; set; }

        public virtual Job Job { get; set; }
        public virtual Finance PaidFrom { get; set; }
        public virtual Section Section { get; set; }
        public virtual ICollection<Timesheet> Timesheet { get; set; }

        //Ručně přidáno
        [Display(Name = "Jméno")]
        public string FullName => Name + " " + Surname;

        [Display(Name = "Číslo bankovního účtu")]
        public string FullBankAccount => $"{BankAccount ?? "XXX"}/{BankCode ?? "XXX"}";

        [Display(Name = "Adresa")]
        public string FullAddress => Street + " " + HouseNumber + ", " + PostalCode + " " + City;
    }
}
