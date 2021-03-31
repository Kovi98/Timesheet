using System;
using System.Collections.Generic;
using System.Text;

namespace Timesheet.Entity.Entities
{
    public partial class Payment
    {
        public bool IsPayed { get { return !(PaymentDateTime is null); } }
        public decimal RewardToPay 
        {
            get
            {
                decimal value = 0;
                foreach (var item in Timesheet)
                {
                    value += item.Reward ?? 0;
                }
                return value;
            }
        }
    }
}
