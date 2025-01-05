using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybank.Database
{
    internal class CashIn
    {
        [Key, AutoIncrement]
        public int CreditId { get; set; }

        public decimal? CreditAmount { get; set; }

        [Required]
        public DateTime CreditDate { get; set; }

        public string CreditDescription { get; set; }

        public int? DebtId { get; set; }

        public int? UserId { get; set; }

        // Foreign key references
        [ForeignKey("DebtId")]
        public Debt Debt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
