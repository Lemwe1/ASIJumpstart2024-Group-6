﻿using System;

namespace ASI.Basecode.Data.Models
{
    public partial class MTransaction
    {
        public int TransactionId { get; set; }
        public int CategoryId { get; set; }
        public int DeLiId { get; set; }
        public int UserId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }

        public virtual MCategory Category { get; set; }
        public virtual MDebitLiab DeLi { get; set; }
        public virtual MUser User { get; set; }
    }
}
