using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class MWallet
    {
        public MWallet()
        {
            MTransactions = new HashSet<MTransaction>();
        }

        public int WalletId { get; set; }
        public string WalletName { get; set; }
        public decimal WalletBalance { get; set; }
        public decimal WalletOriginalBalance { get; set; }
        public string WalletIcon { get; set; }
        public string WalletColor { get; set; }
        public int UserId { get; set; }

        public virtual MUser User { get; set; }
        public virtual ICollection<MTransaction> MTransactions { get; set; }
    }
}
