using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Database
{
    internal class User
    {
        public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserPassword { get; set; }

        /*[Required]*/
        public string CurrencyType { get; set; }

        [EmailAddress]
        public string UserEmail { get; set; }

        public string PasswordSalt { get; set; }
        public string PreferredCurrency { get; set; }

    }
}
