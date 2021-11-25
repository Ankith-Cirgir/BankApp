using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class CustomerAccount
    {
        public List<Transaction> Transactions = new List<Transaction>();


        [MaxLength(45)]
        public string BankId { get; set; }

        [MaxLength(45)]
        public string AccountId { get; set; }

        public float Balance { get; set; }

        [MaxLength(45)]
        public string Name { get; set; }

        [MaxLength(45)]
        public string Password { get; set; }

        [MaxLength(2)]
        public int IsActive { get; set; }

        /*
        public CustomerAccount(string name)
        {
            this.AccountId = $"{name.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyyHHmmss")}";
            this.Name = name;
            this.Balance = 0;
        }*/

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
}
