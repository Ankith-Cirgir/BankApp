using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class Transaction 
    {
        public static int Total { get; set; }

        [MaxLength(45)]
        public string TransactionId { get; set; }

        [MaxLength(45)]
        public string SenderId { get; set; }

        [MaxLength(45)]
        public string ReceiverId { get; set; }

        public float Amount { get; set; }

        public int Type { get; set; }

        [MaxLength(45)]
        public string Time { get; set; }


        /*
        public Transaction(string TransactionId,string senderId, string receiverId, float amount, int type, string time)
        {
            this.TransactionId = TransactionId;
            this.SenderId = senderId;
            this.ReceiverId = receiverId;
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
        }*/
        public enum TransactionType
        {
            Withdraw = 1,
            Deposit,
            Transfer,
        }

    }
}
