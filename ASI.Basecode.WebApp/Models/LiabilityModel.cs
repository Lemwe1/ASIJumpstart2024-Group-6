using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class LiabilityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }

}
