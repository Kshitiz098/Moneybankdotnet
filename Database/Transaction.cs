using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Database
{
    internal class Transaction
    {
        public int TransactionId { get; set; }


        public string? Note { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string TranactionTitle { get; set; }
        public decimal Amount { get; set; }
        public string TransactionLabel { get; set; }
        public string TagName { get; set; }

        public bool IsEditingNote { get; set; }
        public string UserName { get; set; }
    }
}
