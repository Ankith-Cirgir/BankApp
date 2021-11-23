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

        public void init() {
            sqlHandler = new SQLHandler();
            sqlHandler.init();

            sqlHandler.AddCurrency("INR",1,"Mon09112021");
        }


        public string AddBank(string name, float sRTGS, float sIMPS, float oRTGS, float oIMPS)
        {
            return sqlHandler.AddBank(name, sRTGS, sIMPS, oRTGS, oIMPS);
        }

        public string AddBank(string name)
        {
            return sqlHandler.AddBank(name);
        }

        public string CreateCustomerAccount(string name, string pass,string bankId)
        {
            return sqlHandler.CreateCustomerAccount(name, pass, bankId);
        }

        public string CreateStaffAccount(string name, string pass, string bankId)
        {
            return sqlHandler.CreateStaffAccount(name, pass, bankId);
        }

        public string DepositAmount(string accountId,float amount, string _currencyName)
        {
            return sqlHandler.DepositAmount(accountId,amount,_currencyName);
        }

        public bool WithdrawAmount(string accountId, float amount)
        {
            return sqlHandler.WithdrawAmount(accountId, amount);
        }

        public bool AuthenticateCustomer(string accountId,string pass)
        {
            return sqlHandler.AuthenticateCustomer(accountId, pass);
        }

        public bool AuthenticateStaff(string accountId, string pass)
        {
            return sqlHandler.Authenticatestaff(accountId,pass);
        }

        public string UpdateCustomerName(string accountId,string newName)
        {
            return sqlHandler.UpdateCustomerName(accountId, newName);
        }

        public string UpdateCustomerPassword(string accountId, string newPassword)
        {
            return sqlHandler.UpdateCustomerPassword(accountId, newPassword);
        }

        public bool DeleteCustomerAccount(string accountId)
        {
            return sqlHandler.DeleteCustomerAccount(accountId);
        }

        public string GetName(string accountId)
        {
            return sqlHandler.GetName(accountId);
        }

        public float GetBalance(string accountId)
        {
            return sqlHandler.GetBalance(accountId);
        }

        
        public bool TransferAmountRTGS(string fromId, string toId, float amount)
        {
            return sqlHandler.TransferAmountRTGS(fromId, toId, amount);
        }

        public bool TransferAmountIMPS(string fromId, string toId, float amount)
        {
            return sqlHandler.TransferAmountIMPS(fromId, toId, amount);
        } 

        public ConsoleTable GetTransactions(string accountId)
        {
            return sqlHandler.GetTransactions(accountId);
        }

        public float UpdatesRTGS(float val, string bankId)
        {
            return sqlHandler.UpdatesRTGS(val,bankId);
        }

        public float UpdatesIMPS(float val, string bankId)
        {
            return sqlHandler.UpdatesIMPS(val, bankId);
        }

        public float UpdateoRTGS(float val, string bankId)
        {
            return sqlHandler.UpdateoRTGS(val, bankId);
        }

        public float UpdateoIMPS(float val, string bankId)
        {
            return sqlHandler.UpdateoIMPS(val, bankId);
        }

        public float GetBankProfits(string bankId)
        {
            return sqlHandler.GetBankProfits(bankId);
        }

        
        public bool RevertTransaction(string transactionId) 
        {
            return sqlHandler.RevertTransaction(transactionId);
        } 

        public bool AddCurrency(string name, float value, string bankId)
        {
            return sqlHandler.AddCurrency(name, value, bankId);
        }

    }

}