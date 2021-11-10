using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class Transaction //Rename variables and methods
    {
        public static int Total { get; set; }
        public string TransactionId { get; set; }
        public string sId { get; set; }
        public string rId { get; set; }
        public float Amount { get; set; }
        public int Type { get; set; } 
        public string Time { get; set; }

        public Transaction(string TransactionId,string sId, string rId, float amount, int type, string time)
        {
            this.TransactionId = TransactionId;
            this.sId = sId;
            this.rId = rId;
            this.Amount = amount;
            this.Type = type;
            this.Time = time;
            Total += 1;
        }
        public Transaction(string TransactionId, string rId, float amount, int type, string time)
        {
            this.TransactionId = TransactionId;
            this.rId = rId;
            this.Amount = amount;
            this.Type = type;
            this.Time = time;
            Total += 1;
        }
    }
}
