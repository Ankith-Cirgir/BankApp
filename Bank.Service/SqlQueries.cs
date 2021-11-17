using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Service
{
    class SqlQueries
    {
        public static string CreateDatabase = "CREATE DATABASE bankapp;";
        public static string CreateBanksTable = "CREATE TABLE `bankapp`.`banks` (`BankId` VARCHAR(45) NOT NULL,`BankName` VARCHAR(45) NULL,`Profits` FLOAT NULL DEFAULT 0,`sRTGSCharge` FLOAT NULL DEFAULT 0,`sIMPSCharge` FLOAT NULL DEFAULT 5,`oRTGSCharge` FLOAT NULL DEFAULT 2,`oIMPSCharge` FLOAT NULL DEFAULT 6,PRIMARY KEY(`BankId`));";
        public static string CreateCustomerAccountsTable = "CREATE TABLE `bankapp`.`customeraccounts` (`AccountId` VARCHAR(45) NOT NULL,`BankId` VARCHAR(45) NULL,`Balance` FLOAT NULL DEFAULT 0,`Name` VARCHAR(45) NOT NULL,`Password` VARCHAR(45) NOT NULL,PRIMARY KEY(`AccountId`));";
        public static string CreateStaffAccountsTable = "CREATE TABLE `bankapp`.`staffaccounts` (`AccountId` VARCHAR(45) NOT NULL,`BankId` VARCHAR(45) NULL,`Name` VARCHAR(45) NOT NULL,`Password` VARCHAR(45) NOT NULL,PRIMARY KEY(`AccountId`));";
        public static string CreateTransactionsTable = "CREATE TABLE `bankapp`.`transactions` (`TransactionId` VARCHAR(45) NOT NULL,`Amount` FLOAT NULL,`Type` INT NOT NULL,`Time` VARCHAR(45) NOT NULL,`SenderId` VARCHAR(45) NULL DEFAULT NULL,`RecieverId` VARCHAR(45) NULL DEFAULT NULL,PRIMARY KEY(`TransactionId`));";

        public static string SelectBanks = "SELECT * FROM Banks";
    }
}
