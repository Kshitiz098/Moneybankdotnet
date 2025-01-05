using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybank.Database
{
    internal class Currency
    {
        [Key]
        public int CurrencyId { get; set; }

        [Required]
        [MaxLength(10)]
        public string CurrencyCode { get; set; }

        public int ExchangeRate { get; set; }

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
