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
        private SQLHandler sqlHandler;
        MyDbContext dbContext;
        public string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }


        public void init() {

            dbContext = new MyDbContext();
        }


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

        public string CreateCustomerAccount(string name, string pass,string bankId)
        {
            var customer = new CustomerAccount
            {
                AccountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}",
                Name = name,
                Password = pass,
                BankId = bankId,
                Balance = 0
            };
            dbContext.Add(customer);
            dbContext.SaveChanges();
            return customer.AccountId;
        }

        public string CreateStaffAccount(string name, string pass, string bankId)
        {
            var staffAccounts= new StaffAccounts
            {
                AccountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}",
                Name = name,
                Password = pass,
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
            GenerateTransactions(accountId, amount, 2);
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
                GenerateTransactions(accountId, amount, 1);
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool AuthenticateCustomer(string accountId,string pass)
        {
            var customer = dbContext.CustomerAccounts.Find(accountId);
            return customer.Password == pass ? true : false;
        }

        public bool AuthenticateStaff(string accountId, string pass)
        {
            var staff = dbContext.StaffAccounts.Find(accountId);
            return staff.Password == pass ? true : false;
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
            return sqlHandler.DeleteCustomerAccount(accountId);
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

        public void GenerateTransactions(string fromId, string toId, float amount, int type)
        {
            var transaction = new Transaction
            {
                TransactionId = $"{dbContext.CustomerAccounts.Find(toId).BankId}{toId}{GetDateTimeNow(true)}",
                Amount = amount,
                Type = type,
                SenderId = fromId,
                ReceiverId = toId,
                Time = GetDateTimeNow(false)
            };
            dbContext.Transaction.Add(transaction);
            dbContext.SaveChanges();
        }

        public void GenerateTransactions(string toId, float amount, int type)
        {
            var transaction = new Transaction
            {
                TransactionId = $"{dbContext.CustomerAccounts.Find(toId).BankId}{toId}{GetDateTimeNow(true)}",
                Amount = amount,
                Type = type,
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
                _ = toCustomer.BankId == fromCustomer.BankId ? toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(toCustomer.BankId).sRTGSCharge): toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(fromCustomer.BankId).oRTGSCharge);
                GenerateTransactions(fromId,toId,amount,3);
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
                _ = toCustomer.BankId == fromCustomer.BankId ? toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(toCustomer.BankId).sIMPSCharge) : toCustomer.Balance = toBalance + amount - (amount * dbContext.Banks.Find(fromCustomer.BankId).oIMPSCharge);
                GenerateTransactions(fromId, toId, amount, 3);
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

        public float UpdatesRTGS(float val, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.sRTGSCharge = val;
            dbContext.SaveChanges();
            return val;
        }

        public float UpdatesIMPS(float val, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.sIMPSCharge = val;
            dbContext.SaveChanges();
            return val;
        }

        public float UpdateoRTGS(float val, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.oRTGSCharge = val;
            dbContext.SaveChanges();
            return val;
        }

        public float UpdateoIMPS(float val, string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            bank.oIMPSCharge = val;
            dbContext.SaveChanges();
            return val;
        }

        public float GetBankProfits(string bankId)
        {
            var bank = dbContext.Banks.Find(bankId);
            return bank.Profits;
        }

        
        public bool RevertTransaction(string transactionId) 
        {
            return sqlHandler.RevertTransaction(transactionId);
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