using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class Transaction 
    {
        public static int Total { get; set; }
        public string TransactionId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public float Amount { get; set; }
        public int Type { get; set; } 
        public string Time { get; set; }

        public Transaction(string TransactionId,string sId, string rId, float amount, int type, string time)
        {
            this.TransactionId = TransactionId;
            this.SenderId = sId;
            this.ReceiverId = rId;
            this.Amount = amount;
            this.Type = type;
            this.Time = time;
            Total += 1;
        }
        public Transaction(string TransactionId, string rId, float amount, int type, string time)
        {
            this.TransactionId = TransactionId;
            this.ReceiverId = rId;
            this.Amount = amount;
            this.Type = type;
            this.Time = time;
            Total += 1;
        }
        public enum TransactionType
        {
            Withdraw = 1,
            Deposit,
            Transfer,
        }

    }
}
