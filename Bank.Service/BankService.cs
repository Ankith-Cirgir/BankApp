using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Model;
using ConsoleTables;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace BankApp.Service
{
    public class BankService
    {
        private SQLHandler sqlHandler;

        public BankService(IConfiguration configuration)
        {
            sqlHandler = new SQLHandler(configuration);
        }

        public static string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public string AddBank(string name, float sRTGS, float sIMPS, float oRTGS, float oIMPS)
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId",bankId));
            parameterList.Add(new MySqlParameter("@BankName", name));
            parameterList.Add(new MySqlParameter("@sRTGSCharge", sRTGS));
            parameterList.Add(new MySqlParameter("@sIMPSCharge", sIMPS));
            parameterList.Add(new MySqlParameter("@oRTGSCharge", oRTGS));
            parameterList.Add(new MySqlParameter("@oIMPSCharge", oIMPS));
            parameterList.Add(new MySqlParameter("@Profits", 0));
            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoBanksTable, sqlParameters: parameterList);
            return bankId;
        }

        public string AddBank(string name)
        {
            string bankId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            parameterList.Add(new MySqlParameter("@BankName", name));
            parameterList.Add(new MySqlParameter("@sRTGSCharge", 0));
            parameterList.Add(new MySqlParameter("@sIMPSCharge", 5));
            parameterList.Add(new MySqlParameter("@oRTGSCharge", 2));
            parameterList.Add(new MySqlParameter("@oIMPSCharge", 6));
            parameterList.Add(new MySqlParameter("@Profits", 0)); 
            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoBanksTable, sqlParameters: parameterList);

            return bankId;
        }

        public string CreateCustomerAccount(string name, string pass, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";

            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            parameterList.Add(new MySqlParameter("@Password", pass));
            parameterList.Add(new MySqlParameter("@Name", name));
            parameterList.Add(new MySqlParameter("@Balance", 0));

            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoCustomersTable, sqlParameters: parameterList);
            return accountId;
        }

        public string CreateStaffAccount(string name, string pass, string bankId)
        {
            string accountId = $"{name.Substring(0, 3)}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            parameterList.Add(new MySqlParameter("@Name", name));
            parameterList.Add(new MySqlParameter("@Password", pass));


            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoStaffsTable, sqlParameters: parameterList);
            return accountId;
        }

        public bool UpdateBalance(string accountId, float newBalance)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@newBalance", newBalance));
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateBalance, sqlParameters: parameterList);
            return true;
        }

        public void GenerateTransaction(string receiverId, float amount, Transaction.TransactionType type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", TId));
            parameterList.Add(new MySqlParameter("@Amount", amount));
            parameterList.Add(new MySqlParameter("@Type", (int)type));
            parameterList.Add(new MySqlParameter("@Time", GetDateTimeNow(false)));
            parameterList.Add(new MySqlParameter("@ReceiversId", receiverId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertTransactionReceiver, parameterList);

        }

        public void GenerateTransaction(string senderId, string receiverId, float amount, Transaction.TransactionType type)
        {
            string TId = $"{GetBankId(receiverId)}{receiverId}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", TId));
            parameterList.Add(new MySqlParameter("@Amount", amount));
            parameterList.Add(new MySqlParameter("@Type", type));
            parameterList.Add(new MySqlParameter("@Time", GetDateTimeNow(false)));
            parameterList.Add(new MySqlParameter("@ReceiverId", receiverId));
            parameterList.Add(new MySqlParameter("@SenderId", senderId));
            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertTransaction, sqlParameters: parameterList);
        }

        public string DepositAmount(string accountId, float amount, string currencyName)
        {
            try
            {
                float currentBalance = GetBalance(accountId);
                float newBalance = currentBalance + amount * GetCurrencyValue(currencyName);

                UpdateBalance(accountId, newBalance);
                GenerateTransaction(accountId, amount, Transaction.TransactionType.Deposit);
                return GetName(accountId);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public bool WithdrawAmount(string accountId, float amount)
        {
            try
            {
                float newBalance = GetBalance(accountId) - amount;
                if (newBalance < 0)
                {
                    return false;
                }
                UpdateBalance(accountId, newBalance);
                GenerateTransaction(accountId, amount, Transaction.TransactionType.Withdraw);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AuthenticateCustomer(string accountId, string pass)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@Password", pass));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.AuthenticateCustomer, sqlParameters: parameterList);
            return x != null && x.ToString() == accountId;
        }

        public bool AuthenticateStaff(string accountId, string pass)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@AccountId", accountId));
                parameterList.Add(new MySqlParameter("@Password", pass));

                var x = sqlHandler.ExecuiteScaler(SqlQueries.AuthenticateStaff, sqlParameters: parameterList);

                return x!= null && x.ToString() == accountId ;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public int UpdateCustomerName(string accountId, string newName)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newName));

            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateName, sqlParameters: parameterList);
        }

        public int UpdateCustomerPassword(string accountId, string newPassword)
        {

            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newPassword));
            
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatePassword, sqlParameters: parameterList); 
        }

        public bool DeleteCustomerAccount(string accountId)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@AccountId", accountId));
    
                return sqlHandler.ExecuiteNonQuery(SqlQueries.DeleteCustomerAccount, sqlParameters: parameterList) == 1;
            }
            catch
            {
                return false;
            }
        }

        public string GetName(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            return sqlHandler.ExecuiteScaler(SqlQueries.GetName, sqlParameters: parameterList).ToString();
        }

        public float GetBalance(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            return float.Parse(sqlHandler.ExecuiteScaler(SqlQueries.GetBalance, sqlParameters: parameterList).ToString());
        }

        public bool TransferAmountRTGS(string fromId, string toId, float amount)
        {
            float UpdatedAmount;
            string fromBankId = GetBankId(fromId);
            if (GetBalance(fromId) - amount < 0)
            {
                return false;
            }
            if (fromBankId == GetBankId(toId))
            {
                float temp = amount * (GetBanksRTGSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (GetBankoRTGSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }

            GenerateTransaction(fromId, toId, amount, Transaction.TransactionType.Transfer);

            UpdateBalance(fromId, GetBalance(fromId) - amount);
            UpdateBalance(toId, GetBalance(toId) + UpdatedAmount);

            UpdateBankProfits(fromBankId, GetBankProfits(fromBankId) + (amount - UpdatedAmount));
            return true;
        }

        public bool TransferAmountIMPS(string fromId, string toId, float amount)
        {
            float UpdatedAmount;
            string fromBankId = GetBankId(fromId);

            if (GetBalance(fromId) - amount < 0)
            {
                return false;
            }
            if (fromBankId == GetBankId(toId))
            {
                float temp = amount * (GetBanksIMPSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }
            else
            {
                float temp = amount * (GetBankoIMPSCharges(fromBankId) / 100);
                UpdatedAmount = amount - temp;
            }

            GenerateTransaction(fromId, toId, amount, Transaction.TransactionType.Transfer);

            UpdateBalance(fromId, GetBalance(fromId) - amount);
            UpdateBalance(toId, GetBalance(toId) + UpdatedAmount);

            UpdateBankProfits(fromBankId, GetBankProfits(fromBankId) + (amount - UpdatedAmount));

            return true;
        }

        public ConsoleTable GetTransactions(string accountId)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@ReceiverId", accountId));
                parameterList.Add(new MySqlParameter("@SenderId", accountId));

                DataTable dt = sqlHandler.ExecuteReader(SqlQueries.GetTransactions, parameterList);
                ConsoleTable table = new ConsoleTable(new ConsoleTableOptions { Columns = new[] { "TransactionId", "SendersAccountId", "RecieversAccountId", "Type", "Amount", "Time" }, EnableCount = false });

                foreach (DataRow dr in dt.Rows)
                {
                    string type = "";
                    switch (dr["Type"])
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
                    table.AddRow(dr["TransactionId"], dr["SenderId"], dr["ReceiverId"], type, dr["Amount"], dr["Time"]);
                }
                return table;
            }
            catch
            {
                return new ConsoleTable();
            }
        }

        public int UpdatesRTGS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sRTGSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesRTGS, sqlParameters: parameterList);
        }

        public int UpdatesIMPS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sIMPSCharge", currencyValue));

            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesIMPS, sqlParameters: parameterList);
        }

        public int UpdateoRTGS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oRTGSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoRTGS, sqlParameters: parameterList);
        }

        public int UpdateoIMPS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oIMPSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoIMPS, sqlParameters: parameterList); 
        }

        public float GetBanksRTGSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var sRTGSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBanksRTGSCharges, sqlParameters: parameterList);
            return float.Parse(sRTGSValue.ToString());
        }
        public float GetBanksIMPSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var sIMPSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBanksIMPSCharges, sqlParameters: parameterList);
            return float.Parse(sIMPSValue.ToString());
        }
        public float GetBankoRTGSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var oRTGSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBankoRTGSCharges, sqlParameters: parameterList);
            return float.Parse(oRTGSValue.ToString());
        }
        public float GetBankoIMPSCharges(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var oIMPSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBankoIMPSCharges, sqlParameters: parameterList);
            return float.Parse(oIMPSValue.ToString());
        }

        public float GetBankProfits(string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", bankId));

            var bankProfits = sqlHandler.ExecuiteScaler(SqlQueries.GetBankProfits, sqlParameters: parameterList);
            return float.Parse(bankProfits.ToString());
        }
        public bool UpdateBankProfits(string bankId, float amount)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@newProfits", amount));
                parameterList.Add(new MySqlParameter("@BankId", bankId));

                sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateBankProfits, parameterList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetBankId(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            return sqlHandler.ExecuiteScaler(SqlQueries.GetBankId, sqlParameters: parameterList).ToString();
        }

        public bool AddCurrency(string name, float value, string bankId)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@BankId", bankId));
                parameterList.Add(new MySqlParameter("@Currency", name));
                parameterList.Add(new MySqlParameter("@Value", value));

                sqlHandler.ExecuiteNonQuery(SqlQueries.AddCurrency, sqlParameters: parameterList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public float GetCurrencyValue(string currencyName)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@CurrencyName", currencyName));

            var x = sqlHandler.ExecuiteScaler(SqlQueries.GetCurrencyValue, sqlParameters: parameterList);
            return float.Parse(x.ToString());
        }

        public bool RevertTransaction(string transactionId)
        {
            try
            {
                float transactionAmount = GetTransactionAmount(transactionId);
                string receiversId = GetTransactionReceiverId(transactionId);
                float receiversBalance = GetBalance(receiversId);

                switch (GetTransactionType(transactionId))
                {
                    case (int)Transaction.TransactionType.Deposit:
                        if (receiversBalance - transactionAmount >= 0)
                        {
                            float newBalance = receiversBalance - transactionAmount;
                            UpdateBalance(receiversId, newBalance);
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    case (int)Transaction.TransactionType.Withdraw:
                        UpdateBalance(receiversId, receiversBalance + transactionAmount);
                        break;

                    case (int)Transaction.TransactionType.Transfer:
                        string sendersId = GetTransactionSenderId(transactionId);

                        float newBal = receiversBalance - transactionAmount;
                        UpdateBalance(receiversId, newBal);

                        float sendersBalance = GetBalance(sendersId);
                        UpdateBalance(sendersId, sendersBalance + transactionAmount);
                        break;

                }
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

                sqlHandler.ExecuiteNonQuery(SqlQueries.DeleteTransaction, sqlParameters: parameterList);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public float GetTransactionAmount(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var transactionAmount = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionAmount, sqlParameters: parameterList);
            return float.Parse(transactionAmount.ToString());
        }

        public int GetTransactionType(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var transactionType = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionType, sqlParameters: parameterList);
            return int.Parse(transactionType.ToString());
        }

        public string GetTransactionSenderId(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var senderId = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionSenderId, sqlParameters: parameterList);
            return senderId.ToString();
        }

        public string GetTransactionReceiverId(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));

            var receiverId = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionReceiverId, sqlParameters: parameterList);
            return receiverId.ToString();
        }
    }
}