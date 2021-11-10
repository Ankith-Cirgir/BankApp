using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class Account
    {
        public List<Transaction> Transactions = new List<Transaction>();
        public string BankId { get; set; }
        public string AccountId { get; set; }
        public float Balance { get; set; }
        public string Name { get; set; }
        public string Passowrd { get; set; }

        public Account(string name)
        {
            this.AccountId = name.Substring(0, 3) + DateTime.Now.ToString("ddMMyyyyHHmmss");
            this.Name = name;
            this.Balance = 0;
        }

        public List<Transaction> GetTransactions()
        {
            return Transactions;
        }

        public bool SetTransaction(Transaction t)
        {
            Transactions.Add(t);
            return true;
        }
    }

    public class StaffAccount
    {
        public string AccountId { get; set; }

        public string Name { get; set; }
        
        public string Passowrd { get; set; }

        public StaffAccount(string accountID, string name, string password)
        {
            this.AccountId = accountID;
            this.Name = name;
            this.Passowrd = password;
        }
    }
}
