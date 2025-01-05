using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybank.Database
{
    internal class Debt
    {
        [Key]
        public int DebtId { get; set; }

        public int? DebtAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public string DeptSource { get; set; }

        public int? IsPending { get; set; }
        public int? TransactionId { get; set; }

        // Navigation properties
        public ICollection<CashIn> Credits { get; set; } = new List<CashIn>();

        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }
    }
}
