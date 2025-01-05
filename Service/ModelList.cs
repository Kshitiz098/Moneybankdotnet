using Moneybank.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybank.Service
{
    internal class ModelList
    {




        public List<Database.Transaction> Transactions { get; set; } = new List<Database.Transaction>();
        public List<Tag> Tagss { get; set; } = new List<Tag>();
        public List<User> Users { get; set; } = new List<User>();
        public List<CashIn> credits { get; set; } = new List<CashIn>();

        public List<CashOut> debits { get; set; } = new List<CashOut>();

        public List<Debt> debts { get; set; } = new List<Debt>();
        public List<Currency> currency { get; set; } = new List<Currency>();

        public List<UserData> userfiles { get; set; } = new List<UserData>();



    }
}