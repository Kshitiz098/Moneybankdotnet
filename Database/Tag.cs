using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Database
{
    internal class Tag
    {
        public int TagId { get; set; }

        [Required]

        public string TagName { get; set; }
    }
}
