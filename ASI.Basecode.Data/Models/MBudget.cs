using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class MBudget
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public decimal MonthlyBudget { get; set; }
        public decimal RemainingBudget { get; set; }

        public virtual MCategory Category { get; set; }
        public virtual MUser User { get; set; }
    }
}
