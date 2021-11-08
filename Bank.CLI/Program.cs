using System;
using System.Collections.Generic;
using BankApp.Service;
using ConsoleTables;

namespace BankApp.CLI
{

    public partial class Program //PARTIAL CLASSES
        // GET STRING (DONE)
        // try use ? : where ever possible (DONE)
        // change variable names according to the context (DONE)
        // StandardMessages to messages (DONE)
        //return bool insted of string (DONE)
    {
        static void Main(string[] args)
        {
            bool exit = false;

            BankService bankService = new BankService();
            println(bankService.init());
            Console.ReadLine();

            while (!exit)
            {
                Console.Clear();
                try {
                    MainMenu mainMenu = (MainMenu)Enum.Parse(typeof(MainMenu), GetString(Messages.WelcomeMenu)); // TRY PARSE
                    switch (mainMenu)
                    {
                        case MainMenu.Deposit:
                            Console.Clear();
                            string depositId = GetString(Messages.AskAccountID);

                            string currency = GetString("Enter the first 3 letter if the currency: ");

                            int amount = GetNumber(Messages.AskDepositAmount);
                            string depositName = bankService.DepositAmount(depositId, amount, currency);
                            Console.Clear();
                            println($"{amount}₹ have been deposited into {depositName} Account");
                            GetString("");
                            break;


                        case MainMenu.Login:
                            Console.Clear();
                            string loginId, loginPassword;

                            int option = GetNumber("1) Bankstaff Login\n2) Customer Login\n\nEnter Your Choice: ");

                            Console.Clear();
                            loginId = GetString(Messages.AskAccountID);

                            loginPassword = GetString(Messages.AskPassword);
                            Console.Clear();

                            if (option == 1)
                            {
                                if (bankService.AuthenticateStaff(loginId, loginPassword))
                                {
                                    bool e = false;
                                    while (!e)
                                    {
                                        Console.Clear();
                                        StaffLoginMenu StaffLoginMenu = (StaffLoginMenu)Enum.Parse(typeof(StaffLoginMenu), GetString(Messages.StaffLoginMenu));
                                        switch (StaffLoginMenu)
                                        {
                                            case StaffLoginMenu.CreateAccount:
                                                Console.Clear();
                                                string createAccountName = GetString(Messages.AskName);
                                                string createAccountPassword = GetString(Messages.AskPassword);
                                                string createAccountId = bankService.CreateCustomerAccount(createAccountName, createAccountPassword);
                                                Console.Clear();
                                                println($"Bank account created with:\nAccount ID: {createAccountId}\nPassword:{createAccountPassword}");
                                                Console.Read();
                                                break;

                                            case StaffLoginMenu.UpdateAccount:
                                                Console.Clear();
                                                UpdateCustomerAccountMenu updateCustomerAccountLoginChoice = (UpdateCustomerAccountMenu)Enum.Parse(typeof(UpdateCustomerAccountMenu), GetString(Messages.UpdateCustomerAccount));
                                                switch (updateCustomerAccountLoginChoice)
                                                {
                                                    case UpdateCustomerAccountMenu.UpdateName:
                                                        Console.Clear();
                                                        string CustomerAccountID = GetString(Messages.AskAccountID);
                                                        string NewName = GetString(Messages.AskName);
                                                        NewName = bankService.UpdateCustomerName(CustomerAccountID, NewName);
                                                        println($"Name has been updated to {NewName}");
                                                        Console.Read();
                                                        break;
                                                    case UpdateCustomerAccountMenu.UpdatePassword:
                                                        Console.Clear();
                                                        string CustomerID = GetString(Messages.AskAccountID);
                                                        string NewPassword = GetString(Messages.AskPassword);
                                                        NewPassword = bankService.UpdateCustomerPassword(CustomerID, NewPassword);
                                                        print($"Password has been updated to {NewPassword}");
                                                        Console.Read();
                                                        break;
                                                    case UpdateCustomerAccountMenu.Back: //USE DEFAULT
                                                        Console.Clear();
                                                        break;
                                                }
                                                break;
                                            case StaffLoginMenu.DeleteAccount:
                                                Console.Clear();
                                                string customerAccountId = GetString(Messages.AskAccountID);
                                                println(bankService.DeleteCustomerAccount(customerAccountId) ? "Account Found and Deleted !!!" : "Account not found in records...");
                                                break;
                                            case StaffLoginMenu.AddCurrency:
                                                Console.Clear();
                                                string currencyName = GetString("Enter 3 letter name of the new currency: ");
                                                float rate = GetNumber($"Enter the number of rupees per one {currencyName}: ");
                                                bankService.AddCurrency(currencyName,rate);
                                                break;
                                            case StaffLoginMenu.UpdatesRTGS:
                                                string bankID = GetString("Enter Bank ID:");
                                                int newsRTGS = GetNumber("Enter New sRTGS value: ");
                                                int temp = bankService.UpdatesRTGS(newsRTGS,bankID);
                                                print($"sRTGS updated to {temp}");
                                                Console.ReadLine();
                                                break;
                                            case StaffLoginMenu.UpdatesIMPS:
                                                bankID = GetString("Enter Bank ID:");
                                                int newsIMPS = GetNumber("Enter New sRTGS value: ");
                                                temp = bankService.UpdatesRTGS(newsIMPS, bankID);
                                                print($"sRTGS updated to {temp}");
                                                Console.ReadLine();
                                                break;
                                            case StaffLoginMenu.UpdateoRTGS:
                                                bankID = GetString("Enter Bank ID:");
                                                int newoRTGS = GetNumber("Enter New sRTGS value: ");
                                                temp = bankService.UpdatesRTGS(newoRTGS, bankID);
                                                print($"sRTGS updated to {temp}");
                                                Console.ReadLine();
                                                break;
                                            case StaffLoginMenu.UpdateoIMPS:
                                                bankID = GetString("Enter Bank ID:");
                                                int newoIMPS = GetNumber("Enter New sRTGS value: ");
                                                temp = bankService.UpdatesRTGS(newoIMPS, bankID);
                                                print($"sRTGS updated to {temp}");
                                                Console.ReadLine();
                                                break;
                                            case StaffLoginMenu.ViewAccountTransaction:
                                                Console.Clear();
                                                string id = GetString(Messages.AskAccountID);
                                                ConsoleTable cTable = bankService.GetTransactions(id);
                                                cTable.Write();
                                                print("\nPress Enter to exit...");
                                                Console.Read();
                                                break;
                                            case StaffLoginMenu.RevertTransaction:
                                                Console.Clear();
                                                print("Enter the TransactionID: ");
                                                string transactionId = Console.ReadLine();
                                                Console.Clear();
                                                println(bankService.RevertTransaction(transactionId) ? "Transaction successfully reverted !!!" : "Insufficient funds...");
                                                Console.ReadLine();
                                                break;
                                            case StaffLoginMenu.Logout:
                                                e = true;
                                                break;
                                            case StaffLoginMenu.ShowBankProfits:
                                                bankID = GetString("Enter BankID: ");
                                                print(bankService.ShowBankProfits(bankID).ToString());
                                                Console.ReadLine();
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    println(Messages.InvalidCredentials);
                                    Console.Read();
                                }
                            }
                            else
                            {

                                if (bankService.AuthenticateCustomer(loginId, loginPassword))
                                {
                                    bool e = false;
                                    while (!e)
                                    {
                                        Console.Clear();
                                        string loginName = bankService.GetName(loginId);
                                        float balance = bankService.GetBalance(loginId);
                                        println($"Welcome {loginName}");
                                        println($"Your account balance is {balance}₹");


                                        CustomerLoginMenu loginChoice = (CustomerLoginMenu)Enum.Parse(typeof(CustomerLoginMenu), GetString(Messages.LoginMenu));

                                        switch (loginChoice)
                                        {
                                            case CustomerLoginMenu.TransferMoney:
                                                string ID_TO = GetString(Messages.TransferAskID);
                                                int transferAmount = GetNumber(Messages.AskTransferAmount);
                                                Console.Clear();
                                                println(bankService.TransferAmount(loginId, ID_TO, transferAmount) ? Messages.TransactionSuccess : Messages.TransactionErrorInsufficientBal);
                                                Console.Read();
                                                break;
                                            case CustomerLoginMenu.Withdraw:
                                                int a = GetNumber(Messages.AskWithdrawAmount);

                                                bool check = bankService.WithdrawAmount(loginId, a);
                                                Console.Clear();
                                                println(check ? $"{a} has been withdrawed succesfully" : Messages.InsuffiecientFunds);
                                                Console.Read();

                                                break;
                                            case CustomerLoginMenu.ShowTransactions:
                                                Console.Clear();
                                                ConsoleTable t = bankService.GetTransactions(loginId);
                                                t.Write();
                                                print("\nPress Enter to exit...");
                                                Console.Read();
                                                break;
                                            case CustomerLoginMenu.Logout:
                                                e = true;
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.Clear();
                                    print(Messages.InvalidCredentials);
                                    Console.Read();
                                }
                            }
                            break;
                        case MainMenu.EXIT:
                            exit = true;
                            break;
                    }
                }
                catch
                {
                    Console.Clear();
                    println("Error Occured");
                    Console.ReadLine();
                }
                
            }
        }

        

    }
}