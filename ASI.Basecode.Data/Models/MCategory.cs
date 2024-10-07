using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class MCategory
    {
        public MCategory()
        {
            MTransactions = new HashSet<MTransaction>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public int UserId { get; set; }

        public virtual MUser User { get; set; }
        public virtual ICollection<MTransaction> MTransactions { get; set; }
    }
}
