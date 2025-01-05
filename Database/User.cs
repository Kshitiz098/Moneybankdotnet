using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybank.Database
{
    internal class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserPassword { get; set; }

        [Required]
        [MaxLength(50)]
        public string CurrencyType { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        // Navigation properties
        public ICollection<CashIn> Credits { get; set; } = new List<CashIn>();
        public ICollection<CashOut> Debits { get; set; } = new List<CashOut>();
        public ICollection<UserData> Files { get; set; } = new List<UserData>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
