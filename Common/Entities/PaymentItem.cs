using System;
using System.Collections.Generic;

namespace Timesheet.Common
{
    public class PaymentItem
    {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime CreateTime { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public decimal Reward { get; set; }
        public decimal Tax { get; set; }
        public decimal RewardToPay { get; set; }
        public decimal Hours { get; set; }
        public ICollection<Timesheet> Timesheet { get; set; }
        public Payment Payment { get; set; }
        public int PaymentId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
