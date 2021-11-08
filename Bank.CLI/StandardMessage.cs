using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.CLI
{
    class Messages //change name MESSAGES
    {
        public static string WelcomeMenu = "Choose an option...\n1) Deposit Amount\n2) Login\n3) EXIT\n\nEnter your choice: ";
        

        public static string StaffLoginMenu = "Choose an action...\n1) Create Account\n2) Update Account\n3) Delete Account\n4) Add Currency\n5) Update sRTGS\n6) Update sIMPS\n7) Update oRTGS\n8) Update oIMPS\n9) View Account Transaction\n10) Revert Transaction\n11) Logout\n12) Show Bank Profits\n\nEnter your choice: ";


        public static string UpdateCustomerAccount = "Choose an action...\n1) Update Name\n2) Update Password\n3) Back\n\nEnter your choice: ";
        

        public static string AskName = "Enter name: ";
        

        public static string AskPassword = "Enter Password: ";
        

        public static string AskAccountID = "Enter the AccountID : ";
        

        public static string AskDepositAmount = "Enter the amount to be deposited: ";
        

        public static string AskWithdrawAmount = "Enter the amount to withdraw: ";
        

        public static string InvalidAccountID = "Invlid Account ID !!!";


        public static string LoginMenu = "\n\n1) Transfer Money (INR only)\n2) Withdraw Money (INR only)\n3) Show Transactions\n4) Logout\n\nChoose an option:";


        public static string InvalidCredentials = "Wrong ID or Password...";


        public static string TransferAskID = "Enter the ID of the account to whome u want to transfer ammount: ";


        public static string AskTransferAmount = "Enter the amount to be transfered: ";


        public static string TransactionSuccess = "Transaction sucessfully completed !!!";
        

        public static string TransactionErrorInsufficientBal = "Unable to complete transaction as there is insufficient balance.";
        

        public static string InsuffiecientFunds = "Insufficient Funds.....";
        

        public static string WithdrawError = "An error occured while withdrawing...";
        

        public static string TransactionFetchingError = "An error occured while fetching Transactions...";
        
    }
}
