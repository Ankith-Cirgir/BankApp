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
        public string TransactionID { get; set; }
        public string sID { get; set; }
        public string rID { get; set; }
        public float Amount { get; set; }
        public int Type { get; set; } 
        public string Time { get; set; }

        public Transaction(string TID,string sID, string rID, float amount, int type, string time)
        {
            this.TransactionID = TID;
            this.sID = sID;
            this.rID = rID;
            this.Amount = amount;
            this.Type = type;
            this.Time = time;
            Total += 1;
        }
        public Transaction(string TID, string rID, float amount, int type, string time)
        {
            this.TransactionID = TID;
            this.rID = rID;
            this.Amount = amount;
            this.Type = type;
            this.Time = time;
            Total += 1;
        }
    }
}
