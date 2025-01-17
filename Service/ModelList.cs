using MoneyBanks.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Service
{
    internal class ModelList
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Tag> Tagss { get; set; } = new List<Tag>();
        public List<User> Users { get; set; } = new List<User>();
        public List<Inflow> credits { get; set; } = new List<Inflow>();

        public List<OutFlow> debits { get; set; } = new List<OutFlow>();

        public List<Debt> debts { get; set; } = new List<Debt>();
        public List<Currency> currency { get; set; } = new List<Currency>();


    }
}
