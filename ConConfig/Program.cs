using System;
using System.Configuration;
using System.Collections.Specialized;

namespace ConConfig
{
    class Program
    {
        static string ConnectionSting = ConfigurationManager.AppSettings.Get("ConnectionString");

        static void Main(string[] args)
        {
        }
    }
}
