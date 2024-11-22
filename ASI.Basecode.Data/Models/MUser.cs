using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class MUser
    {
        public MUser()
        {
            MBudgets = new HashSet<MBudget>();
            MCategories = new HashSet<MCategory>();
            MTransactions = new HashSet<MTransaction>();
            MWallets = new HashSet<MWallet>();
        }

        public int UserId { get; set; }
        public string InsBy { get; set; }
        public DateTime InsDt { get; set; }
        public string UpdBy { get; set; }
        public DateTime UpdDt { get; set; }
        public bool Deleted { get; set; }
        public string UserCode { get; set; }
        public string Password { get; set; }
        public string TemporaryPassword { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string LastNameKana { get; set; }
        public string FirstNameKana { get; set; }
        public string Mail { get; set; }
        public int? UserRole { get; set; }
        public string Remarks { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiration { get; set; }
        public bool IsVerified { get; set; }
        public string VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiration { get; set; }

        public virtual ICollection<MBudget> MBudgets { get; set; }
        public virtual ICollection<MCategory> MCategories { get; set; }
        public virtual ICollection<MTransaction> MTransactions { get; set; }
        public virtual ICollection<MWallet> MWallets { get; set; }
    }
}
