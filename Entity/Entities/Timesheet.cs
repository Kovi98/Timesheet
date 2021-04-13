﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Timesheet.Entity.Entities
{
    public partial class Timesheet
    {
        public int Id { get; set; }

        [Display(Name = "Hodiny", Description = "Vykázaný čas")]
        public decimal? Hours { get; set; }

        [Display(Name = "Trenér")]
        [Required(ErrorMessage = "Trenér je povinný!")]
        public int PersonId { get; set; }
        [Display(Name = "Práce")]
        [Required(ErrorMessage = "Práce je povinná!")]
        public int JobId { get; set; }

        [Display(Name = "Vytvořeno", Description = "Datum a čas vytvoření záznamu")]
        public DateTime CreateTime { get; set; }

        public byte[] RowVersion { get; set; }

        [Display(Name = "Od", Description = "Začátek")]
        public DateTime? DateTimeFrom { get; set; }

        [Display(Name = "Do", Description = "Konec")]
        public DateTime? DateTimeTo { get; set; }

        [Required(ErrorMessage = "Název je povinný!")]
        public string Name { get; set; }

        [Display(Name = "Platba")]
        public int? PaymentId { get; set; }

        [Display(Name = "Odměna", Description = "Odměna za práci")]
        [DataType(DataType.Currency)]
        public decimal? Reward { get; set; }

        [Display(Name = "Daň", Description = "Srážková daň")]
        [DataType(DataType.Currency)]
        public decimal Tax { get; set; }

        public virtual Job Job { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Person Person { get; set; }

        //Ručně přidáno
        [DataType(DataType.Currency)]
        [Display(Name = "K vyplacení", Description = "Odměna k vyplacení")]
        public decimal ToPay { get { return (Reward ?? 0) - Tax; } }

        [Display(Name = "Hodiny", Description = "Vykázaný čas")]
        public TimeSpan HoursTime
        {
            get
            {
                return TimeSpan.FromHours(Hours.HasValue ? (double)Hours.Value : 0);
            }
        }
    }
}
