using System;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public int CategoryId { get; set; }
        public int DeLiId { get; set; }
        public int UserId { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; }
    }
}
