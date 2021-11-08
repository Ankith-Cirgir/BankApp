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

        string BankID;
        
        private Dictionary<string, Account> _customerAccounts = new Dictionary<string, Account>();

        private Dictionary<string, StaffAccount> _staffAccounts = new Dictionary<string, StaffAccount>();

        private Dictionary<string, Transaction> _transactions = new Dictionary<string, Transaction>();

        private Dictionary<string, float> _currency = new Dictionary<string, float>();

        public string init() {
            string bankName = "MoneyBank";
            string bankID = this.AddBank(bankName); 
            this.CreateStaffAccount("admin", "admin");
            this._currency.Add("INR",1);
            return bankID;
        }


        public string AddBank(string Name, int sRTGS, int sIMPS, int oRTGS, int oIMPS)
        {
            Bank bank = new Bank(Name, sRTGS, sIMPS, oRTGS, oIMPS);
            this._banks.Add(bank.ID, bank);
            return bank.ID;
        }

        public string AddBank(string Name)
        {
            Bank bank = new Bank(Name);
            this._banks.Add(bank.ID, bank);
            this.BankID = bank.ID;
            return bank.ID;
        }

        public string CreateCustomerAccount(string Name, string pass)
        {
            Account acc = new Account(Name);
            acc.Passowrd = pass;
            acc.bankID = _banks[this.BankID].ID;
            string AccountID = acc.AccountID;
            _customerAccounts.Add(AccountID,acc);
            return AccountID;
        }

        public string CreateStaffAccount(string Name, string pass)
        {
            StaffAccount acc = new StaffAccount("admin",Name,pass);
            acc.Passowrd = pass;
            acc.AccountID = "admin";
            _staffAccounts.Add("admin",acc);
            return "admin";
        }

        public string DepositAmount(string AccountID,int Amount, string _currencyName)
        {
            Account acc = _customerAccounts[AccountID];
            acc.balance = acc.balance + Amount*(_currency[_currencyName]);
            string TID = acc.bankID + acc.AccountID + DateTime.Now.ToString("HHmmss");
            Transaction tr = new Transaction(TID, AccountID, Amount, "Deposit", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            this._transactions.Add(TID, tr);
            acc.SetTransaction(tr);
            return acc.Name;
        }

        public bool WithdrawAmount(string AccountID, int Amount)
        {
            Account acc = _customerAccounts[AccountID];
            float bal = acc.balance;
            if (bal < Amount)
            {
                return false;
            }

            acc.balance = bal - Amount;
            string TID = acc.bankID + acc.AccountID + DateTime.Now.ToString("HHmmss");
            Transaction tr = new Transaction(TID, AccountID, Amount, "Withdraw", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            acc.SetTransaction(tr);
            this._transactions.Add(TID, tr);
            return true;
        }

        public bool AuthenticateCustomer(string AccountID,string pass)
        {
            Account acc = _customerAccounts[AccountID];
            if (acc.Passowrd == pass) // direct return
            {
                return true;
            }
            return false;
        }

        public bool AuthenticateStaff(string AccountID, string pass)
        {
            StaffAccount acc = _staffAccounts[AccountID];
            if (acc.Passowrd == pass) // direct return
            {
                return true;
            }
            return false;
        }

        public string UpdateCustomerName(string AccountID,string NewName)
        {
            Account acc = _customerAccounts[AccountID];
            acc.Name = NewName;
            return NewName;
        }

        public string UpdateCustomerPassword(string AccountID, string NewPassword)
        {
            Account acc = _customerAccounts[AccountID];
            acc.Passowrd = NewPassword;
            return NewPassword;
        }

        public bool DeleteCustomerAccount(string AccountID)
        {
            return _customerAccounts.Remove(AccountID);
        }

        public string GetName(string AccountID)
        {
            return _customerAccounts[AccountID].Name;
        }

        public float GetBalance(string AccountID)
        {
            return _customerAccounts[AccountID].balance;
        }

        public bool TransferAmount(string ID_FROM, string ID_TO, int amount)
        {
            float UpdatedAmount;
            Account acc_from = _customerAccounts[ID_FROM];
            Account acc_to = _customerAccounts[ID_TO];

            if (acc_from.balance - amount < 0)
            {
                return false;
            }
            if(acc_from.bankID == acc_to.bankID)
            {
                float temp = amount * (_banks[acc_from.bankID].sRTGSCharge / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (_banks[acc_from.bankID].oRTGSCharge / 100);
                UpdatedAmount = amount - temp;
            }

            string TID_FROM = acc_from.bankID + acc_from.AccountID + DateTime.Now.ToString("HHmmss");
            acc_from.SetTransaction(new Transaction(TID_FROM, acc_from.AccountID, acc_to.AccountID, amount, "Transfer", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
            string TID_TO = acc_to.bankID + acc_to.AccountID + DateTime.Now.ToString("HHmmss");
            acc_to.SetTransaction(new Transaction(TID_TO, acc_from.AccountID, acc_to.AccountID, UpdatedAmount, "Transfer", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));

            this._transactions.Add(TID_FROM, new Transaction(TID_FROM,acc_from.AccountID,acc_to.AccountID,amount,"Transfer", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"))); // create a function for DateTime.Now

            _banks[acc_from.bankID].Profits += amount - UpdatedAmount; //EXTRA
            acc_from.balance -= amount;
            acc_to.balance += UpdatedAmount;

            return true;
        }

        public ConsoleTable GetTransactions(string AccountID)
        {
            Account acc = _customerAccounts[AccountID];
            List<Transaction> _transactions = acc.GetTransactions();
            ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = new[] { "SNO","TransactionID", "SendersAccountID", "RecieversAccountID", "Type", "Amount", "Time" }, EnableCount = false });
            foreach(Transaction transaction in _transactions)
            {
                table.AddRow(transaction.SNO, transaction.TransactionID, transaction.sID, transaction.rID, transaction.desc, transaction.amount, transaction.time);
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
            //
            if (transaction.sID == null)
            {
                Account from = _customerAccounts[transaction.rID];
                if (transaction.desc == "Deposit")
                {

                    if (from.balance < transaction.amount)
                    {
                        return false;
                    }

                    from.balance -= transaction.amount;
                }
                from.transactions.Remove(from.transactions[from.transactions.FindIndex(item => item.TransactionID == TID)]);
                return true;
            }
            else if(transaction.rID == null)
            {
                Account to = _customerAccounts[transaction.sID];
                to.balance += transaction.amount;
                to.transactions.Remove(to.transactions[to.transactions.FindIndex(item => item.TransactionID == TID)]);
                return true;

            }
            Account From = _customerAccounts[transaction.sID];
            Account To = _customerAccounts[transaction.rID];

            if (To.balance < transaction.amount)
            {
                return false;
            }
            From.balance += transaction.amount;
            To.balance -= transaction.amount;

            From.transactions.Remove(From.transactions[From.transactions.FindIndex(item => item.TransactionID == TID)]);
            To.transactions.Remove(To.transactions[To.transactions.FindIndex(item => item.TransactionID == TID)]);

            _transactions.Remove(TID);
            return true;
        }

        public bool AddCurrency(string name, float value)
        {
            _currency.Add(name,value);
            return true;
        }

    }
}