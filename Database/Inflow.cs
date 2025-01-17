using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Database
{
    internal class Inflow
    {
        public int CashInId { get; set; }

        public decimal CashInAmount { get; set; }

        [Required]
        public DateTime CashInDate { get; set; }

        public string CashInDescription { get; set; }

        public string TagName { get; set; }

        public string Title { get; set; }

        public string UserName { get; set; }
    }
}
