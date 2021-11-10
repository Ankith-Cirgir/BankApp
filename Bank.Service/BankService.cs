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
        // used string interpolation whereever possible
        // created GetDateTimeNow funtion to reuse code
    {
        public string GetDateTimeNow(bool forId)
        {
            if (forId)
            {
                return DateTime.Now.ToString("ddMMyyyyHHmmss");
            }
            else
            {
                return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        

        private Dictionary<string, Bank> _banks = new Dictionary<string, Bank>();
        
        private Dictionary<string, Account> _customerAccounts = new Dictionary<string, Account>();

        private Dictionary<string, StaffAccount> _staffAccounts = new Dictionary<string, StaffAccount>();

        private Dictionary<string, Transaction> _transactions = new Dictionary<string, Transaction>();

        private Dictionary<string, float> _currency = new Dictionary<string, float>();

        string bankId;
        public string init() {
            string bankName = "MoneyBank";
            string bankId = this.AddBank(bankName); 
            this.CreateStaffAccount("admin", "admin");
            this._currency.Add("INR",1);
            return bankId;
        }


        public string AddBank(string name, int sRTGS, int sIMPS, int oRTGS, int oIMPS)
        {
            Bank bank = new Bank(name, sRTGS, sIMPS, oRTGS, oIMPS);
            this._banks.Add(bank.Id, bank);
            return bank.Id;
        }

        public string AddBank(string name)
        {
            Bank bank = new Bank(name);
            this._banks.Add(bank.Id, bank);
            this.bankId = bank.Id;
            return bank.Id;
        }

        public string CreateCustomerAccount(string name, string pass)
        {
            Account acc = new Account(name);
            acc.Passowrd = pass;
            acc.BankId = _banks[this.bankId].Id;
            string accountId = acc.AccountId;
            _customerAccounts.Add(accountId,acc);
            return accountId;
        }

        public string CreateStaffAccount(string name, string pass)
        {
            StaffAccount acc = new StaffAccount("admin",name,pass);
            acc.Passowrd = pass;
            acc.AccountId = "admin";
            _staffAccounts.Add("admin",acc);
            return "admin";
        }

        public string DepositAmount(string accountId,int amount, string _currencyName)
        {
            Account acc = _customerAccounts[accountId];
            acc.Balance = acc.Balance + amount*(_currency[_currencyName]);
            string TId = $"{acc.BankId}{acc.AccountId}{GetDateTimeNow(true)}";
            Transaction tr = new Transaction(TId, accountId, amount, 2, GetDateTimeNow(false));
            this._transactions.Add(TId, tr);
            acc.SetTransaction(tr);
            return acc.Name;
        }

        public bool WithdrawAmount(string accountId, int amount)
        {
            Account acc = _customerAccounts[accountId];
            float bal = acc.Balance;
            if (bal < amount)
            {
                return false;
            }

            acc.Balance = bal - amount;
            string TId = $"{acc.BankId}{acc.AccountId}{GetDateTimeNow(true)}";
            Transaction tr = new Transaction(TId, accountId, amount, 1, GetDateTimeNow(false));
            acc.SetTransaction(tr);
            this._transactions.Add(TId, tr);
            return true;
        }

        public bool AuthenticateCustomer(string accountId,string pass)
        {
            Account acc = _customerAccounts[accountId];
            return acc.Passowrd == pass ?  true :  false;
        }

        public bool AuthenticateStaff(string accountId, string pass)
        {
            StaffAccount acc = _staffAccounts[accountId];
            return acc.Passowrd == pass ? true : false;
        }

        public string UpdateCustomerName(string accountId,string newName)
        {
            Account acc = _customerAccounts[accountId];
            acc.Name = newName;
            return newName;
        }

        public string UpdateCustomerPassword(string accountId, string newPassword)
        {
            Account acc = _customerAccounts[accountId];
            acc.Passowrd = newPassword;
            return newPassword;
        }

        public bool DeleteCustomerAccount(string accountId)
        {
            return _customerAccounts.Remove(accountId);
        }

        public string GetName(string accountId)
        {
            return _customerAccounts[accountId].Name;
        }

        public float GetBalance(string accountId)
        {
            return _customerAccounts[accountId].Balance;
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
            if(acc_from.BankId == acc_to.BankId)
            {
                float temp = amount * (_banks[acc_from.BankId].sRTGSCharge / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (_banks[acc_from.BankId].oRTGSCharge / 100);
                UpdatedAmount = amount - temp;
            }

            string TIdFrom = $"{acc_from.BankId}{acc_from.AccountId}{GetDateTimeNow(true)}";
            acc_from.SetTransaction(new Transaction(TIdFrom, acc_from.AccountId, acc_to.AccountId, amount, 3, GetDateTimeNow(false)));
            string TIdTo = $"{acc_to.BankId}{acc_to.AccountId}{GetDateTimeNow(true)}";
            acc_to.SetTransaction(new Transaction(TIdTo, acc_from.AccountId, acc_to.AccountId, UpdatedAmount, 3, GetDateTimeNow(false)));

            this._transactions.Add(TIdFrom, new Transaction(TIdFrom,acc_from.AccountId,acc_to.AccountId,amount,3, GetDateTimeNow(false)));

            _banks[acc_from.BankId].Profits += amount - UpdatedAmount; //EXTRA
            acc_from.Balance -= amount;
            acc_to.Balance += UpdatedAmount;

            return true;
        }

        public ConsoleTable GetTransactions(string accountId)
        {
            Account acc = _customerAccounts[accountId];
            List<Transaction> _transactions = acc.GetTransactions();
            ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = new[] { "TransactionId", "SendersAccountId", "RecieversAccountId", "Type", "Amount", "Time" }, EnableCount = false });
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
                table.AddRow( transaction.TransactionId, transaction.sId, transaction.rId, type, transaction.Amount, transaction.Time);
            }
            return table;
        }

        public int UpdatesRTGS(int val, string bankId)
        {
            Bank bank = _banks[bankId];
            bank.sRTGSCharge = val;
            return val;
        }

        public int UpdatesIMPS(int val, string bankId)
        {
            Bank bank = _banks[bankId];
            bank.sRTGSCharge = val;
            return val;
        }

        public int UpdateoRTGS(int val, string bankId)
        {
            Bank bank = _banks[bankId];
            bank.sRTGSCharge = val;
            return val;
        }

        public int UpdateoIMPS(int val, string bankId)
        {
            Bank bank = _banks[bankId];
            bank.sRTGSCharge = val;
            return val;
        }

        public float ShowBankProfits(string bankId)
        {
            Bank bank = _banks[bankId];
            return bank.Profits;
        }

        public bool RevertTransaction(string transactionId) 
        {
            Transaction transaction = _transactions[transactionId];
            Account from = null;
            Account to = null;
            
            if (transaction.sId == null)
            {
                from = _customerAccounts[transaction.rId];
                if (transaction.Type == (int) TransactionType.Deposit)
                {

                    if (from.Balance < transaction.Amount)
                    {
                        return false;
                    }

                    from.Balance -= transaction.Amount;
                }
                from.Transactions.Remove(from.Transactions[from.Transactions.FindIndex(item => item.TransactionId == transactionId)]);
                return true;
            }
            else if(transaction.rId == null)
            {
                to = _customerAccounts[transaction.sId];
                to.Balance += transaction.Amount;
                to.Transactions.Remove(to.Transactions[to.Transactions.FindIndex(item => item.TransactionId == transactionId)]);
                return true;

            }
            from = _customerAccounts[transaction.sId];
            to = _customerAccounts[transaction.rId];

            if (to.Balance < transaction.Amount)
            {
                return false;
            }
            from.Balance += transaction.Amount;
            to.Balance -= transaction.Amount;

            from.Transactions.Remove(from.Transactions[from.Transactions.FindIndex(item => item.TransactionId == transactionId)]);
            to.Transactions.Remove(to.Transactions[to.Transactions.FindIndex(item => item.TransactionId == transactionId)]);

            _transactions.Remove(transactionId);
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