using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.CLI
{
    public partial class Program
    {
        public static void ClearScreen()
        {
            Console.Clear();
        }

        public static string GetString(string message) 
        {
            print(message);
            return Console.ReadLine();
        }

        public static void print(string s)
        {
            Console.Write(s);
        }

        public static void println(string s)
        {
            Console.WriteLine(s);
        }


        public static int GetNumber(string message)
        {
            while (true)
            {
                int Number;
                try
                {
                    print(message);
                    Number = Convert.ToInt32(Console.ReadLine());
                    return Number;
                }
                catch
                {
                    println("Only numbers are accepted");
                }
            }


        }
    }
}
