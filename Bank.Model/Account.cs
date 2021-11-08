using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class Account
    {
        public List<Transaction> transactions = new List<Transaction>();

        public string bankID { get; set; }

        public string AccountID { get; set; }
        public float balance { get; set; }
        public string Name { get; set; }
        public string Passowrd { get; set; }

        public Account(string name)
        {
            this.AccountID = name.Substring(0, 3) + DateTime.Now.ToString("ddMMyyyyHHmmss");
            this.Name = name;
            this.balance = 0;
        }

        public List<Transaction> GetTransactions()
        {
            return transactions;
        }

        public bool SetTransaction(Transaction t)
        {
            transactions.Add(t);
            return true;
        }
    }

    public class StaffAccount
    {
        public string AccountID { get; set; }

        public string Name { get; set; }
        
        public string Passowrd { get; set; }

        public StaffAccount(string accountID, string name, string password)
        {
            this.AccountID = accountID;
            this.Name = name;
            this.Passowrd = password;
        }
    }
}
