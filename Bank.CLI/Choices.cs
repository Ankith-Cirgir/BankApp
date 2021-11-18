using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.CLI // add enum for type of transaction
{

    public enum MainMenu
    {
        Deposit = 1,
        Login,
        EXIT,
    }

    public enum CustomerLoginMenu 
    {
        TransferMoney = 1,
        Withdraw,
        ShowTransactions,
        Logout,
    }
    public enum StaffLoginMenu
    {
        CreateAccount = 1,
        UpdateAccount,
        DeleteAccount,
        AddCurrency,
        UpdatesRTGS,
        UpdatesIMPS,
        UpdateoRTGS,
        UpdateoIMPS,
        ViewAccountTransaction,
        RevertTransaction,
        Logout,
        ShowBankProfits,
    }

    public enum UpdateCustomerAccountMenu
    {
        UpdateName = 1,
        UpdatePassword,
        Back,
    }

    public enum TransactionModeMenu
    {
        RTGS=1,
        IMPS,
    }
}
