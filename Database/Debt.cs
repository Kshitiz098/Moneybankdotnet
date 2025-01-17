using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Database
{
    internal class Debt
    {
        public int DebtId { get; set; }

        public decimal DebtsAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public DateTime DebtDate { get; set; }

        public string DeptSource { get; set; }

        public string DeptDescription { get; set; }

        public bool IsPending { get; set; }

        public decimal AmountCleared { get; set; }

        public string TagName { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
    }
}
