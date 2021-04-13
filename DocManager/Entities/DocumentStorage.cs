using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.DocManager.Entities
{
    public partial class DocumentStorage
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Název je povinný!")]
        [Display(Name = "Název", Description = "Název šablony")]
        public string Name { get; set; }
        public byte[] DocumentSource { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name = "Vytvořeno", Description = "Datum a čas vytvoření záznamu")]
        public DateTime CreateTime { get; set; }
        [Display(Name = "Soubor", Description = "Název souboru")]
        public string DocumentName { get; set; }
        [Display(Name = "Primární", Description = "Šablona je primární pro generování smlouvy")]
        public bool IsDefault { get; set; }
    }
}
