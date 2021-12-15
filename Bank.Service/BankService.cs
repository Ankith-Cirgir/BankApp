using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Model;
using ConsoleTables;

namespace BankApp.Service
{
    public class BankService
    {

        private MyDbContext dbContext;

        public BankService()
        {
            dbContext = new MyDbContext();
        }

        private string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Use this method to create a new bank with all the charges and a name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sRTGS"></param>
        /// <param name="sIMPS"></param>
        /// <param name="oRTGS"></param>
        /// <param name="oIMPS"></param>
        /// <returns>You get BankId of the bank you just created.</returns>
        public string AddBank(string name, float sRTGS, float sIMPS, float oRTGS, float oIMPS)
        {
            var bank = new Bank
            {
                BankId = $"{name.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyy")}",
                BankName = name,
                sRTGSCharge = sRTGS,
                sIMPSCharge = sIMPS,
                oRTGSCharge = oRTGS,
                oIMPSCharge = oIMPS
            };
            dbContext.Add(bank);
            dbContext.SaveChanges();
            return bank.BankId;
        }

        /// <summary>
        /// Use this method to create a new bank with just a name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>You get BankId of the bank you just created.</returns>
        public string AddBank(string name)
        {
            var bank = new Bank
            {
                BankId = $"{name.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyy")}",
                BankName = name,
                sRTGSCharge = 0,
                sIMPSCharge = 5,
                oRTGSCharge = 2,
                oIMPSCharge = 6
            };
            dbContext.Add(bank);
            dbContext.SaveChanges();
            return bank.BankId;
        }

        public string CreateCustomerAccount(string name, string password,string bankId)
        {
            var customer = new CustomerAccount
            {
                AccountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}",
                Name = name,
                Password = password,
                BankId = bankId,
                Balance = 0
            };
            dbContext.Add(customer);
            dbContext.SaveChanges();
            return customer.AccountId;
        }

        public string CreateStaffAccount(string name, string password, string bankId)
        {
            var staffAccounts= new StaffAccounts
            {
                AccountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}",
                Name = name,
                Password = password,
                BankId = bankId
            };
            dbContext.Add(staffAccounts);
            dbContext.SaveChanges();
            return staffAccounts.AccountId;
        }

        public string DepositAmount(string accountId,float amount, string _currencyName)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            float currentBalance = customer.Balance;
            customer.Balance = currentBalance + amount;
            dbContext.SaveChanges();
            GenerateTransactions(accountId, amount, Transaction.TransactionType.Deposit);
            return customer.Name;
        }

        public bool WithdrawAmount(string accountId, float amount)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            float currentBalance = customer.Balance;
            if(currentBalance-amount >= 0)
            {
                customer.Balance = currentBalance - amount;
                dbContext.SaveChanges();
                GenerateTransactions(accountId, amount, Transaction.TransactionType.Withdraw);
                return true;
            }
            return false;
        }

        public bool AuthenticateCustomer(string accountId,string password)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            return customer.Password == password;
        }

        public bool AuthenticateStaff(string accountId, string password)
        {
            var staff = dbContext.StaffAccounts.Find(accountId);
            return staff.Password == password;
        }

        public string UpdateCustomerName(string accountId,string newName)
        {

            var customer = dbContext.CustomerAccounts.Find(accountId);
            customer.Name = newName;
            dbContext.SaveChanges();
            return newName;
        }

        public string UpdateCustomerPassword(string accountId, string newPassword)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            customer.Password = newPassword;
            dbContext.SaveChanges();
            return newPassword;
        }

        public bool DeleteCustomerAccount(string accountId)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            dbContext.Remove(customer);
            dbContext.SaveChanges();
            return true;
        }

        public string GetName(string accountId)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            return customer.Name;
        }

        public float GetBalance(string accountId)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            return customer.Balance;
        }

        public void GenerateTransactions(string fromId, string toId, float amount, Transaction.TransactionType type)
        {
            var transaction = new Transaction
            {
                TransactionId = $"{dbContext.CustomerAccounts.Find(toId).BankId}{toId}{GetDateTimeNow(true)}",
                Amount = amount,
                Type = (int)type,
                SenderId = fromId,
                ReceiverId = toId,
                Time = GetDateTimeNow(false)
            };
            dbContext.Transaction.Add(transaction);
            dbContext.SaveChanges();
        }

        public void GenerateTransactions(string toId, float amount, Transaction.TransactionType type)
        {
            var transaction = new Transaction
            {
                TransactionId = $"{dbContext.CustomerAccounts.Find(toId).BankId}{toId}{GetDateTimeNow(true)}",
                Amount = amount,
                Type = (int)type,
                ReceiverId = toId,
                Time = GetDateTimeNow(false)
            };
            dbContext.Transaction.Add(transaction);
            dbContext.SaveChanges();
        }


        public bool TransferAmountRTGS(string fromId, string toId, float amount)
        {
            var fromCustomer = dbContext.CustomerAccounts.Find(fromId);
            var toCustomer = dbContext.CustomerAccounts.Find(toId);

            if(fromCustomer.Balance - amount >= 0)
            {
                float fromBalance = fromCustomer.Balance;
                fromCustomer.Balance = fromBalance - amount;
                float toBalance = toCustomer.Balance;
                if (toCustomer.BankId == fromCustomer.BankId) 
                {
                    toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(toCustomer.BankId).sRTGSCharge);
                }
                else 
                {
                    toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(fromCustomer.BankId).oRTGSCharge); 
                }
                GenerateTransactions(fromId,toId,amount,Transaction.TransactionType.Transfer);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool TransferAmountIMPS(string fromId, string toId, float amount)
        {
            var fromCustomer = dbContext.CustomerAccounts.Find(fromId);
            var toCustomer = dbContext.CustomerAccounts.Find(toId);

            if (fromCustomer.Balance - amount >= 0)
            {
                float fromBalance = fromCustomer.Balance;
                fromCustomer.Balance = fromBalance - amount;
                float toBalance = toCustomer.Balance;
                if (toCustomer.BankId == fromCustomer.BankId)
                {
                    toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(toCustomer.BankId).sIMPSCharge);
                }
                else
                {
                    toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(fromCustomer.BankId).oIMPSCharge);
                }
                GenerateTransactions(fromId, toId, amount, Transaction.TransactionType.Transfer);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public ConsoleTable GetTransactions(string accountId)
        {
            ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = new[] { "TransactionId", "SendersAccountId", "RecieversAccountId", "Type", "Amount", "Time" }, EnableCount = false });
            var transactions = dbContext.Transaction.Where(e => e.ReceiverId == accountId || e.ReceiverId == accountId);
            foreach (Transaction transaction in transactions){
                string type = "";
                switch (transaction.Type)
                {
                    case (int)Transaction.TransactionType.Deposit:
                        type = "Deposit";
                        break;
                    case (int)Transaction.TransactionType.Transfer:
                        type = "Transfer";
                        break;
                    case (int)Transaction.TransactionType.Withdraw:
                        type = "Withdraw";
                        break;
                }
                table.AddRow(transaction.TransactionId, transaction.SenderId, transaction.ReceiverId, type, transaction.Amount, transaction.Time);
            }
            
            return table;
        }

        public bool UpdatesRTGS(float sRTGSCharge, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.sRTGSCharge = sRTGSCharge;
            dbContext.SaveChanges();
            return true;
        }

        public bool UpdatesIMPS(float sIMPSCharge, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.sIMPSCharge = sIMPSCharge;
            dbContext.SaveChanges();
            return true;
        }

        public bool UpdateoRTGS(float oRTGSCharge, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.oRTGSCharge = oRTGSCharge;
            dbContext.SaveChanges();
            return true;
        }

        public bool UpdateoIMPS(float oIMPSCharge, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.oIMPSCharge = oIMPSCharge;
            dbContext.SaveChanges();
            return true;
        }

        public float GetBankProfits(string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            return bank.Profits;
        }

        public void DeleteTransaction(string TransactionId)
        {
            dbContext.Remove(dbContext.Transaction.Find(TransactionId));
            dbContext.SaveChanges();
        }

        public bool RevertTransaction(string transactionId) 
        {
            var transaction = dbContext.Transaction.Find(transactionId);
            string senderId = transaction.SenderId;
            string receiverId = transaction.ReceiverId;
            var receiverCustomer = dbContext.CustomerAccounts.Find(receiverId);
            float amount = transaction.Amount;
            switch(transaction.Type)

            {
                case (int)Transaction.TransactionType.Withdraw:
                    receiverCustomer.Balance += amount;
                    dbContext.SaveChanges();
                    DeleteTransaction(transactionId);
                    return true;

                case (int)Transaction.TransactionType.Deposit:
                    if (receiverCustomer.Balance - amount >= 0)
                    {
                        receiverCustomer.Balance -= amount;
                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;

                case (int)Transaction.TransactionType.Transfer:
                    var senderCustomer = dbContext.CustomerAccounts.Find(senderId);
                    if(receiverCustomer.Balance - amount >= 0)
                    {
                        senderCustomer.Balance += amount;
                        receiverCustomer.Balance -= amount;
                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;
            }
            return false;
        } 

        public bool AddCurrency(string name, float value, string bankId)
        {
            var currency = new Currency
            {
                currency = name,
                value = value,
                BankId = bankId
            };
            dbContext.Currency.Add(currency);
            dbContext.SaveChanges();
            return true;
        }

    }

}