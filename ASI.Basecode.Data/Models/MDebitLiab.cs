﻿using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class MDebitLiab
    {
        public MDebitLiab()
        {
            MTransactions = new HashSet<MTransaction>();
        }

        public int DeLiId { get; set; }
        public string DeLiType { get; set; }
        public decimal DeLiBalance { get; set; }
        public string DeLiIcon { get; set; }
        public string DeLiColor { get; set; }
        public DateTime? DeLiHapp { get; set; }
        public DateTime? DeLiDue { get; set; }
        public string DeLiName { get; set; }
        public int UserId { get; set; }

        public virtual MUser User { get; set; }
        public virtual ICollection<MTransaction> MTransactions { get; set; }
    }
}
