using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Service
{
    class SqlQueries
    {
        public static string CheckTabelsExist = "SELECT count(*) FROM information_schema.tables WHERE table_schema = 'bankapp' AND table_name = 'banks' LIMIT 1;";


        public static string CreateDatabase = "CREATE DATABASE bankapp;";
        public static string CreateBanksTable = "CREATE TABLE `bankapp`.`banks` (`BankId` VARCHAR(45) NOT NULL,`BankName` VARCHAR(45) NULL,`Profits` FLOAT NULL DEFAULT 0,`sRTGSCharge` FLOAT NULL DEFAULT 0,`sIMPSCharge` FLOAT NULL DEFAULT 5,`oRTGSCharge` FLOAT NULL DEFAULT 2,`oIMPSCharge` FLOAT NULL DEFAULT 6,PRIMARY KEY(`BankId`));";
        public static string CreateCustomerAccountsTable = "CREATE TABLE `bankapp`.`customeraccounts` ( `AccountId` VARCHAR(45) NOT NULL, `BankId` VARCHAR(45) NOT NULL, `Balance` FLOAT NOT NULL, `Name` VARCHAR(45) NOT NULL, `Password` VARCHAR(45) NOT NULL, `IsActive` TINYINT NOT NULL DEFAULT 1, PRIMARY KEY (`AccountId`), INDEX `Customer BankId_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `Customer BankId` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateStaffAccountsTable = "CREATE TABLE `bankapp`.`staffaccounts` ( `AccountId` VARCHAR(45) NOT NULL, `BankId` VARCHAR(45) NOT NULL, `Name` VARCHAR(45) NOT NULL, `Password` VARCHAR(45) NOT NULL, PRIMARY KEY (`AccountId`), INDEX `staff and bankId link_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `staff and bankId link` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateTransactionsTable = "CREATE TABLE `bankapp`.`transactions` ( `TransactionId` VARCHAR(45) NOT NULL, `Amount` FLOAT NULL, `Type` INT NULL, `Time` VARCHAR(45) NULL, `SenderId` VARCHAR(45) NULL, `ReceiverId` VARCHAR(45) NULL, PRIMARY KEY (`TransactionId`), INDEX `SenderId_idx` (`SenderId` ASC) VISIBLE, INDEX `ReceiverId_idx` (`ReceiverId` ASC) VISIBLE, CONSTRAINT `SenderId` FOREIGN KEY (`SenderId`) REFERENCES `bankapp`.`customeraccounts` (`AccountId`) ON DELETE NO ACTION ON UPDATE NO ACTION, CONSTRAINT `ReceiverId` FOREIGN KEY (`ReceiverId`) REFERENCES `bankapp`.`customeraccounts` (`AccountId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateCurrencyTable = "CREATE TABLE `bankapp`.`currency` ( `currency` VARCHAR(4) NOT NULL, `BankId` VARCHAR(45) NULL, PRIMARY KEY (`currency`), INDEX `bank and currency link_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `bank and currency link` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";


        public static string GetBankProfits = "SELECT Profits from `bankapp`.`banks` where BankId = '{0}';";
        public static string GetBalance = "SELECT `Balance` from `bankapp`.`customeraccounts` where AccountId = '{0}';";
        public static string GetName = "SELECT `Name` from `bankapp`.`customeraccounts` where AccountId = '{0}';";
        public static string GetBankId = "SELECT `BankId` from `bankapp`.`customeraccounts` where AccountId = '{0}';";
        public static string GetCurrencyValue = "SELECT `Value` from `bankapp`.`currency` where currency = '{0}'";
        public static string GetTransactions = "SELECT * from `bankapp`.`transactions` WHERE ReceiverId = '{0}' or SenderId = '{1}';";
        public static string GetBanksRTGSCharges = "SELECT sRTGSCharge from `bankapp`.`banks` where BankId = '{0}';";
        public static string GetBanksIMPSCharges = "SELECT sIMPSCharge from `bankapp`.`banks` where BankId = '{0}';";
        public static string GetBankoRTGSCharges = "SELECT oRTGSCharge from `bankapp`.`banks` where BankId = '{0}';";
        public static string GetBankoIMPSCharges = "SELECT oIMPSCharge from `bankapp`.`banks` where BankId = '{0}';";
        public static string GetTransactionAmount = "SELECT Amount from `bankapp`.`transactions` WHERE TransactionId = '{0}'";
        public static string GetTransactionType = "SELECT Type from `bankapp`.`transactions` WHERE TransactionId = '{0}'";
        public static string GetTransactionSenderId = "SELECT SenderId from `bankapp`.`transactions` WHERE transactionId = '{0}';";
        public static string GetTransactionReceiverId = "SELECT ReceiverId from `bankapp`.`transactions` WHERE transactionId = '{0}';";


        public static string InsertIntoCustomersTable = "INSERT INTO `bankapp`.`customeraccounts`(`AccountId`,`BankId`,`Balance`,`Name`,`Password`)VALUES('{0}', '{1}', {2}, '{3}', '{4}');";
        public static string InsertIntoStaffsTable = "INSERT INTO `bankapp`.`staffaccounts`(`AccountId`,`BankId`,`Name`,`Password`)VALUES('{0}', '{1}', '{2}', '{3}');";
        public static string InsertIntoBanksTable = "INSERT INTO `bankapp`.`banks`(`BankId`,`BankName`,`Profits`,`sRTGSCharge`,`sIMPSCharge`,`oRTGSCharge`,`oIMPSCharge`)VALUES('@BankId', '@BankName', @Profits, @sRTGSCharge, @sIMPSCharge, @oRTGSCharge, @oIMPSCharge); ";
        public static string AddCurrency = "INSERT INTO `bankapp`.`currency` (`currency`, `BankId`, `value`) VALUES ('{0}', '{1}', {2});";
        public static string InsertTransactionReceiver = "INSERT INTO `bankapp`.`transactions` (`TransactionId`, `Amount`, `Type`, `Time`, `ReceiverId`) VALUES ('{0}', {1}, {2}, '{3}', '{4}');";
        public static string InsertTransaction = "INSERT INTO `bankapp`.`transactions` (`TransactionId`, `Amount`, `Type`, `Time`, `SenderId`, `ReceiverId`) VALUES ('{0}', {1}, {2}, '{3}', '{4}', '{5}');";


        public static string DeleteTransaction = "DELETE FROM `bankapp`.`transactions` WHERE TransactionId = '{0}';";
        public static string DeleteCustomerAccount = "UPDATE `bankapp`.`customeraccounts` SET  `IsActive` = 0 WHERE `AccountId` = '{0}';";


        public static string UpdateBalance = "UPDATE `bankapp`.`customeraccounts` SET  `Balance` = {0} WHERE `AccountId` = '{1}';";
        public static string UpdatePassword = "UPDATE `bankapp`.`customeraccounts` SET `Password` = '{0}' WHERE `AccountId` = '{1}';";
        public static string UpdateName = "UPDATE `bankapp`.`customeraccounts` SET `Name` = '{0}' WHERE `AccountId` = '{1}';";
        public static string UpdatesRTGS = "UPDATE `bankapp`.`banks` SET `sRTGSCharge` = {0}, WHERE `BankId` = '{1}';";
        public static string UpdatesIMPS = "UPDATE `bankapp`.`banks` SET `sIMPSCharge` = {0}, WHERE `BankId` = '{1}';";
        public static string UpdateoRTGS = "UPDATE `bankapp`.`banks` SET `oRTGSCharge` = {0}, WHERE `BankId` = '{1}';";
        public static string UpdateoIMPS = "UPDATE `bankapp`.`banks` SET `oIMPSCharge` = {0}, WHERE `BankId` = '{1}';";
        public static string UpdateBankProfits = "UPDATE `bankapp`.`banks` SET `Profits` = {0}, WHERE `BankId` = '{1}';";


        public static string AuthenticateCustomer = "SELECT EXISTS(SELECT 1 FROM `bankapp`.`customeraccounts` WHERE AccountId = '{0}' AND Password = '{1}');";
        public static string AuthenticateStaff = "SELECT EXISTS(SELECT 1 FROM `bankapp`.`staffaccounts` WHERE AccountId = '{0}' AND Password = '{1}');";
    }
}