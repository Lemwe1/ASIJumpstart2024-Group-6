using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ASI.Basecode.WebApp.Models
{
    /// <summary>
    /// Login View Model
    /// </summary>
    public class DebitLiabilitiesViewModel
    {
        public List<DebitAccount> DebitAccounts { get; set; }
        public List<Liability> Liabilities { get; set; }
        public decimal TotalNetWorth { get; set; }
    }

}
