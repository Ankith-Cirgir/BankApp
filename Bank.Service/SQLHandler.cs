using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BankApp.Service
{
    class SQLHandler
    {
        private string connStr;
        public void init()
        {
            connStr = "server=localhost;user=root;database=bankapp;port=3306;password=admin";
        }

        public string CreateBanksTable()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(SqlQueries.CreateBanksTable, conn))
                    {
                        cmd.Connection.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        string temp = "";
                        while (reader.Read())
                        {
                            temp += reader.GetString(0);
                        }
                        return temp;
                    }
                }
            }
            catch (Exception e)
            {
                return "SQL ERROR: " + e.ToString();
            }
        }


    }
}
