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
        private Dictionary<string, Bank> _banks = new Dictionary<string, Bank>();
        
        private Dictionary<string, Account> _customerAccounts = new Dictionary<string, Account>();

        private Dictionary<string, StaffAccount> _staffAccounts = new Dictionary<string, StaffAccount>();

        private Dictionary<string, Transaction> _transactions = new Dictionary<string, Transaction>();

        private Dictionary<string, float> _currency = new Dictionary<string, float>();

        string bankID;

        public string init() {
            string bankName = "MoneyBank";
            string bankID = this.AddBank(bankName); 
            this.CreateStaffAccount("admin", "admin");
            this._currency.Add("INR",1);
            return bankID;
        }


        public string AddBank(string name, int sRTGS, int sIMPS, int oRTGS, int oIMPS)
        {
            Bank bank = new Bank(name, sRTGS, sIMPS, oRTGS, oIMPS);
            this._banks.Add(bank.ID, bank);
            return bank.ID;
        }

        public string AddBank(string name)
        {
            Bank bank = new Bank(name);
            this._banks.Add(bank.ID, bank);
            this.bankID = bank.ID;
            return bank.ID;
        }

        public string CreateCustomerAccount(string name, string pass)
        {
            Account acc = new Account(name);
            acc.Passowrd = pass;
            acc.BankID = _banks[this.bankID].ID;
            string accountID = acc.AccountID;
            _customerAccounts.Add(accountID,acc);
            return accountID;
        }

        public string CreateStaffAccount(string name, string pass)
        {
            StaffAccount acc = new StaffAccount("admin",name,pass);
            acc.Passowrd = pass;
            acc.AccountID = "admin";
            _staffAccounts.Add("admin",acc);
            return "admin";
        }

        public string DepositAmount(string accountID,int amount, string _currencyName)
        {
            Account acc = _customerAccounts[accountID];
            acc.Balance = acc.Balance + amount*(_currency[_currencyName]);
            string TID = acc.BankID + acc.AccountID + DateTime.Now.ToString("HHmmss");
            Transaction tr = new Transaction(TID, accountID, amount, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            this._transactions.Add(TID, tr);
            acc.SetTransaction(tr);
            return acc.Name;
        }

        public bool WithdrawAmount(string accountID, int amount)
        {
            Account acc = _customerAccounts[accountID];
            float bal = acc.Balance;
            if (bal < amount)
            {
                return false;
            }

            acc.Balance = bal - amount;
            string TID = acc.BankID + acc.AccountID + DateTime.Now.ToString("HHmmss");
            Transaction tr = new Transaction(TID, accountID, amount, 1, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            acc.SetTransaction(tr);
            this._transactions.Add(TID, tr);
            return true;
        }

        public bool AuthenticateCustomer(string accountID,string pass)
        {
            Account acc = _customerAccounts[accountID];
            if (acc.Passowrd == pass) // direct return
            {
                return true;
            }
            return false;
        }

        public bool AuthenticateStaff(string accountID, string pass)
        {
            StaffAccount acc = _staffAccounts[accountID];
            if (acc.Passowrd == pass) // direct return
            {
                return true;
            }
            return false;
        }

        public string UpdateCustomerName(string accountID,string newName)
        {
            Account acc = _customerAccounts[accountID];
            acc.Name = newName;
            return newName;
        }

        public string UpdateCustomerPassword(string accountID, string newPassword)
        {
            Account acc = _customerAccounts[accountID];
            acc.Passowrd = newPassword;
            return newPassword;
        }

        public bool DeleteCustomerAccount(string accountID)
        {
            return _customerAccounts.Remove(accountID);
        }

        public string GetName(string accountID)
        {
            return _customerAccounts[accountID].Name;
        }

        public float GetBalance(string accountID)
        {
            return _customerAccounts[accountID].Balance;
        }

        public bool TransferAmount(string fromId, string toId, int amount)
        {
            float UpdatedAmount;
            Account acc_from = _customerAccounts[fromId];
            Account acc_to = _customerAccounts[toId];

            if (acc_from.Balance - amount < 0)
            {
                return false;
            }
            if(acc_from.BankID == acc_to.BankID)
            {
                float temp = amount * (_banks[acc_from.BankID].sRTGSCharge / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (_banks[acc_from.BankID].oRTGSCharge / 100);
                UpdatedAmount = amount - temp;
            }

            string TIDFrom = acc_from.BankID + acc_from.AccountID + DateTime.Now.ToString("HHmmss");
            acc_from.SetTransaction(new Transaction(TIDFrom, acc_from.AccountID, acc_to.AccountID, amount, 3, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
            string TIDTo = acc_to.BankID + acc_to.AccountID + DateTime.Now.ToString("HHmmss");
            acc_to.SetTransaction(new Transaction(TIDTo, acc_from.AccountID, acc_to.AccountID, UpdatedAmount, 3, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));

            this._transactions.Add(TIDFrom, new Transaction(TIDFrom,acc_from.AccountID,acc_to.AccountID,amount,3, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"))); // create a function for DateTime.Now

            _banks[acc_from.BankID].Profits += amount - UpdatedAmount; //EXTRA
            acc_from.Balance -= amount;
            acc_to.Balance += UpdatedAmount;

            return true;
        }

        public ConsoleTable GetTransactions(string accountID)
        {
            Account acc = _customerAccounts[accountID];
            List<Transaction> _transactions = acc.GetTransactions();
            ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = new[] { "TransactionID", "SendersAccountID", "RecieversAccountID", "Type", "Amount", "Time" }, EnableCount = false });
            foreach(Transaction transaction in _transactions)
            {
                string type = "";
                switch (transaction.Type)
                {
                    case (int) TransactionType.Deposit:
                        type = "Deposit";
                        break;
                    case (int)TransactionType.Transfer:
                        type = "Transfer";
                        break;
                    case (int)TransactionType.Withdraw:
                        type = "Withdraw";
                        break;
                }
                table.AddRow( transaction.TransactionID, transaction.sID, transaction.rID, type, transaction.Amount, transaction.Time);
            }
            return table;
        }

        public int UpdatesRTGS(int val, string bankID)
        {
            Bank bank = _banks[bankID];
            bank.sRTGSCharge = val;
            return val;
        }

        public int UpdatesIMPS(int val, string bankID)
        {
            Bank bank = _banks[bankID];
            bank.sRTGSCharge = val;
            return val;
        }

        public int UpdateoRTGS(int val, string bankID)
        {
            Bank bank = _banks[bankID];
            bank.sRTGSCharge = val;
            return val;
        }

        public int UpdateoIMPS(int val, string bankID)
        {
            Bank bank = _banks[bankID];
            bank.sRTGSCharge = val;
            return val;
        }

        public float ShowBankProfits(string bankID)
        {
            Bank bank = _banks[bankID];
            return bank.Profits;
        }

        public bool RevertTransaction(string TID) 
        {
            Transaction transaction = _transactions[TID];
            Account from = null;
            Account to = null;
            
            if (transaction.sID == null)
            {
                from = _customerAccounts[transaction.rID];
                if (transaction.Type == (int) TransactionType.Deposit)
                {

                    if (from.Balance < transaction.Amount)
                    {
                        return false;
                    }

                    from.Balance -= transaction.Amount;
                }
                from.Transactions.Remove(from.Transactions[from.Transactions.FindIndex(item => item.TransactionID == TID)]);
                return true;
            }
            else if(transaction.rID == null)
            {
                to = _customerAccounts[transaction.sID];
                to.Balance += transaction.Amount;
                to.Transactions.Remove(to.Transactions[to.Transactions.FindIndex(item => item.TransactionID == TID)]);
                return true;

            }
            from = _customerAccounts[transaction.sID];
            to = _customerAccounts[transaction.rID];

            if (to.Balance < transaction.Amount)
            {
                return false;
            }
            from.Balance += transaction.Amount;
            to.Balance -= transaction.Amount;

            from.Transactions.Remove(from.Transactions[from.Transactions.FindIndex(item => item.TransactionID == TID)]);
            to.Transactions.Remove(to.Transactions[to.Transactions.FindIndex(item => item.TransactionID == TID)]);

            _transactions.Remove(TID);
            return true;
        }

        public bool AddCurrency(string name, float value)
        {
            _currency.Add(name,value);
            return true;
        }

    }

    public enum TransactionType
    {
        Withdraw = 1,
        Deposit,
        Transfer,
    }


}