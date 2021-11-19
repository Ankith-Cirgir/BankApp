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
        public string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        

        private Dictionary<string, Bank> _banks = new Dictionary<string, Bank>();
        
        private Dictionary<string, CustomerAccount> _customerAccounts = new Dictionary<string, CustomerAccount>();

        private Dictionary<string, StaffAccount> _staffAccounts = new Dictionary<string, StaffAccount>();

        private Dictionary<string, Transaction> _transactions = new Dictionary<string, Transaction>();

        private Dictionary<string, float> _currency = new Dictionary<string, float>();
         
        string bankId;
        public string init() {
            string bankName = "MoneyBank";
            string bankId = this.AddBank(bankName); 
            this.CreateStaffAccount("admin", "admin");
            this._currency.Add("INR",1);

            SQLHandler sQLHandler = new SQLHandler();
            sQLHandler.init();

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
            CustomerAccount acc = new CustomerAccount(name);
            acc.Password = pass;
            acc.BankId = _banks[this.bankId].Id;
            string accountId = acc.AccountId;
            _customerAccounts.Add(accountId,acc);
            return accountId;
        }

        public string CreateStaffAccount(string name, string pass)
        {
            StaffAccount acc = new StaffAccount("admin",name,pass);
            acc.Password = pass;
            acc.AccountId = "admin";
            _staffAccounts.Add("admin",acc);
            return "admin";
        }

        public string DepositAmount(string accountId,float amount, string _currencyName)
        {
            CustomerAccount acc = _customerAccounts[accountId];
            acc.Balance = acc.Balance + amount*(_currency[_currencyName]);
            string TId = $"{acc.BankId}{acc.AccountId}{GetDateTimeNow(true)}";
            Transaction tr = new Transaction(TId, accountId, amount, 2, GetDateTimeNow(false));
            this._transactions.Add(TId, tr);
            acc.SetTransaction(tr);
            return acc.Name;
        }

        public bool WithdrawAmount(string accountId, float amount)
        {
            CustomerAccount acc = _customerAccounts[accountId];
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
            CustomerAccount acc = _customerAccounts[accountId];
            return acc.Password == pass ?  true :  false;
        }

        public bool AuthenticateStaff(string accountId, string pass)
        {
            StaffAccount acc = _staffAccounts[accountId];
            return acc.Password == pass ? true : false;
        }

        public string UpdateCustomerName(string accountId,string newName)
        {
            CustomerAccount acc = _customerAccounts[accountId];
            acc.Name = newName;
            return newName;
        }

        public string UpdateCustomerPassword(string accountId, string newPassword)
        {
            CustomerAccount acc = _customerAccounts[accountId];
            acc.Password = newPassword;
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

        public bool TransferAmountRTGS(string fromId, string toId, float amount)
        {
            float UpdatedAmount;
            CustomerAccount acc_from = _customerAccounts[fromId];
            CustomerAccount acc_to = _customerAccounts[toId];

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

            _banks[acc_from.BankId].Profits += amount - UpdatedAmount;
            acc_from.Balance -= amount;
            acc_to.Balance += UpdatedAmount;

            return true;
        }

        public bool TransferAmountIMPS(string fromId, string toId, float amount)
        {
            float UpdatedAmount;
            CustomerAccount acc_from = _customerAccounts[fromId];
            CustomerAccount acc_to = _customerAccounts[toId];

            if (acc_from.Balance - amount < 0)
            {
                return false;
            }
            if (acc_from.BankId == acc_to.BankId)
            {
                float temp = amount * (_banks[acc_from.BankId].sIMPSCharge / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (_banks[acc_from.BankId].oIMPSCharge / 100);
                UpdatedAmount = amount - temp;
            }

            string TIdFrom = $"{acc_from.BankId}{acc_from.AccountId}{GetDateTimeNow(true)}";
            acc_from.SetTransaction(new Transaction(TIdFrom, acc_from.AccountId, acc_to.AccountId, amount, 3, GetDateTimeNow(false)));
            string TIdTo = $"{acc_to.BankId}{acc_to.AccountId}{GetDateTimeNow(true)}";
            acc_to.SetTransaction(new Transaction(TIdTo, acc_from.AccountId, acc_to.AccountId, UpdatedAmount, 3, GetDateTimeNow(false)));

            this._transactions.Add(TIdFrom, new Transaction(TIdFrom, acc_from.AccountId, acc_to.AccountId, amount, 3, GetDateTimeNow(false)));

            _banks[acc_from.BankId].Profits += amount - UpdatedAmount;
            acc_from.Balance -= amount;
            acc_to.Balance += UpdatedAmount;

            return true;
        }

        public ConsoleTable GetTransactions(string accountId)
        {
            CustomerAccount acc = _customerAccounts[accountId];
            List<Transaction> _transactions = acc.GetTransactions();
            ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = new[] { "TransactionId", "SendersAccountId", "RecieversAccountId", "Type", "Amount", "Time" }, EnableCount = false });
            foreach(Transaction transaction in _transactions)
            {
                string type = "";
                switch (transaction.Type)
                {
                    case (int) Transaction.TransactionType.Deposit:
                        type = "Deposit";
                        break;
                    case (int) Transaction.TransactionType.Transfer:
                        type = "Transfer";
                        break;
                    case (int) Transaction.TransactionType.Withdraw:
                        type = "Withdraw";
                        break;
                }
                table.AddRow( transaction.TransactionId, transaction.SenderId, transaction.ReceiverId, type, transaction.Amount, transaction.Time);
            }
            return table;
        }

        public float UpdatesRTGS(float val, string bankId)
        {
            Bank bank = _banks[bankId];
            bank.sRTGSCharge = val;
            return val;
        }

        public float UpdatesIMPS(float val, string bankId)
        {
            Bank bank = _banks[bankId];
            bank.sRTGSCharge = val;
            return val;
        }

        public float UpdateoRTGS(float val, string bankId)
        {
            Bank bank = _banks[bankId];
            bank.sRTGSCharge = val;
            return val;
        }

        public float UpdateoIMPS(float val, string bankId)
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
            CustomerAccount from = null;
            CustomerAccount to = null;
            
            if (transaction.SenderId == null)
            {
                from = _customerAccounts[transaction.ReceiverId];
                if (transaction.Type == (int) Transaction.TransactionType.Deposit)
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
            else if(transaction.ReceiverId == null)
            {
                to = _customerAccounts[transaction.SenderId];
                to.Balance += transaction.Amount;
                to.Transactions.Remove(to.Transactions[to.Transactions.FindIndex(item => item.TransactionId == transactionId)]);
                return true;

            }
            from = _customerAccounts[transaction.SenderId];
            to = _customerAccounts[transaction.ReceiverId];

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

}