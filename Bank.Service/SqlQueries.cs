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
        public static string CreateCustomerAccountsTable = "CREATE TABLE `bankapp`.`customeraccounts` ( `AccountId` VARCHAR(45) NOT NULL, `BankId` VARCHAR(45) NOT NULL, `Balance` FLOAT NULL DEFAULT 0, `Name` VARCHAR(45) NOT NULL, `Password` VARCHAR(45) NOT NULL, PRIMARY KEY (`AccountId`), INDEX `customer bankId link_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `customer bankId link` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateStaffAccountsTable = "CREATE TABLE `bankapp`.`staffaccounts` ( `AccountId` VARCHAR(45) NOT NULL, `BankId` VARCHAR(45) NOT NULL, `Name` VARCHAR(45) NOT NULL, `Password` VARCHAR(45) NOT NULL, PRIMARY KEY (`AccountId`), INDEX `staff and bankId link_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `staff and bankId link` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";
        public static string CreateTransactionsTable = "CREATE TABLE `bankapp`.`transactions` (`TransactionId` VARCHAR(45) NOT NULL,`Amount` FLOAT NULL,`Type` INT NOT NULL,`Time` VARCHAR(45) NOT NULL,`SenderId` VARCHAR(45) NULL DEFAULT NULL,`RecieverId` VARCHAR(45) NULL DEFAULT NULL,PRIMARY KEY(`TransactionId`));";
        public static string CreateCurrencyTable = "CREATE TABLE `bankapp`.`currency` ( `currency` VARCHAR(4) NOT NULL, `BankId` VARCHAR(45) NULL, PRIMARY KEY (`currency`), INDEX `bank and currency link_idx` (`BankId` ASC) VISIBLE, CONSTRAINT `bank and currency link` FOREIGN KEY (`BankId`) REFERENCES `bankapp`.`banks` (`BankId`) ON DELETE NO ACTION ON UPDATE NO ACTION);";


        public static string SelectBanks = "SELECT * FROM Banks";
        public static string GetBankProfits = "SELECT Profits from `bankapp`.`banks` where BankId = '{0}';";
        public static string GetBalance = "SELECT `Balance` from `bankapp`.`customeraccounts` where AccountId = '{0}';";
        public static string GetName = "SELECT `Name` from `bankapp`.`customeraccounts` where AccountId = '{0}';";
        public static string GetBankId = "SELECT `BankId` from `bankapp`.`customeraccounts` where AccountId = '{0}';";
        public static string GetCurrencyValue = "SELECT `Value` from `bankapp`.`currency` where currency = '{0}'";


        public static string InsertIntoCustomersTable = "INSERT INTO `bankapp`.`customeraccounts`(`AccountId`,`BankId`,`Balance`,`Name`,`Password`)VALUES('{0}', '{1}', {2}, '{3}', '{4}');";
        public static string InsertIntoStaffsTable = "INSERT INTO `bankapp`.`staffaccounts`(`AccountId`,`BankId`,`Name`,`Password`)VALUES('{0}', '{1}', '{2}', '{3}');";
        public static string InsertIntoBanksTable = "INSERT INTO `bankapp`.`banks`(`BankId`,`BankName`,`Profits`,`sRTGSCharge`,`sIMPSCharge`,`oRTGSCharge`,`oIMPSCharge`)VALUES('{0}', '{1}', {2}, {3}, {4}, {5}>, {6}); ";
        public static string AddCurrency = "INSERT INTO `bankapp`.`currency` (`currency`, `BankId`, `value`) VALUES ('{0}', '{1}', {2});";
        public static string InsertTransaction = "INSERT INTO `bankapp`.`transactions` (`TransactionId`, `Amount`, `Type`, `Time`, `SenderId`, `ReceiverId`) VALUES ({0}, {1}, {2}, {3}, {4}, {5});";
        

        public static string DeleteCustomerAccount = "DELETE FROM `bankapp`.`customeraccounts` WHERE AccountId = '{0}';";


        public static string UpdateBalance = "UPDATE `bankapp`.`customeraccounts` SET  `Balance` = {0} WHERE `AccountId` = '{1}';";
        public static string UpdatePassword = "UPDATE `bankapp`.`customeraccounts` SET `Password` = '{0}' WHERE `AccountId` = '{1}';";
        public static string UpdateName = "UPDATE `bankapp`.`customeraccounts` SET `Name` = '{0}' WHERE `AccountId` = '{1}';";
        public static string UpdatesRTGS = "UPDATE `bankapp`.`banks` SET `sRTGSCharge` = {0}, WHERE `BankId` = '{1}';";
        public static string UpdatesIMPS = "UPDATE `bankapp`.`banks` SET `sIMPSCharge` = {0}, WHERE `BankId` = '{1}';";
        public static string UpdateoRTGS = "UPDATE `bankapp`.`banks` SET `oRTGSCharge` = {0}, WHERE `BankId` = '{1}';";
        public static string UpdateoIMPS = "UPDATE `bankapp`.`banks` SET `oIMPSCharge` = {0}, WHERE `BankId` = '{1}';";


        public static string AuthenticateCustomer = "SELECT EXISTS(SELECT 1 FROM `bankapp`.`customeraccounts` WHERE AccountId = '{0}');";
        public static string AuthenticateStaff = "SELECT EXISTS(SELECT 1 FROM `bankapp`.`staffaccounts` WHERE AccountId = '{0}');";
    }
}