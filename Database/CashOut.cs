using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybank.Database
{
    internal class CashOut
    {
        [Key]
        public int DebitId { get; set; }

        [Required]
        public DateTime DebitDate { get; set; }

        public decimal DebitAmount { get; set; }
        public string DebitDescription { get; set; }
        public int? UserId { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
