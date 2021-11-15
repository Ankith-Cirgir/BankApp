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


        public static float GetNumber(string message)
        {
            print(message);
            while (true)
            {
                int Number;
                try
                {
                    Number = Convert.ToInt32(Console.ReadLine());
                    return Number;
                }
                catch
                {
                    println("Only numbers are accepted !!");
                    print("Enter: ");
                }
            }


        }
    }
}
