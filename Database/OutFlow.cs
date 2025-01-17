using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Database
{
    internal class OutFlow
    {
        public int CashOutId { get; set; }

        [Required]
        public DateTime CashOutDate { get; set; }

        public decimal CashOutAmount { get; set; }
        public string CashOutDescription { get; set; }

        public string TagName { get; set; }

        public string Title { get; set; }

        public string UserName { get; set; }
    }
}
