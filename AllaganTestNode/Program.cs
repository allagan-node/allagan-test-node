using System;
using System.Reflection;

namespace AllaganTestNode
{
    class Program
    {
        private static string sourceIndexPath = string.Empty;
        private static ExHLanguage sourceLanguage = ExHLanguage.Null;

        private static string targetIndexPath = string.Empty;
        private static ExHLanguage targetLanguage = ExHLanguage.Null;

        private static bool backUpTarget = true;

        static void Main(string[] args)
        {
            while (true)
            {
                PrintScreen();

                string input = Console.ReadLine();

                if (int.TryParse(input, out int _input) && _input > 0 && _input <= 6)
                {

                }
                else
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("");
                }
            }
        }

        static void PrintScreen()
        {
            Console.Clear();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Format("Allagan Test Node v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Pick an operation to execute:");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[1] - Pick source file: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(string.IsNullOrEmpty(sourceIndexPath) ? "NOT SELECTED" : sourceIndexPath);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[2] - Pick source language code: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(sourceLanguage == ExHLanguage.Null ? "NOT SELECTED" : sourceLanguage.ToString());

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[3] - Pick target file: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(string.IsNullOrEmpty(targetIndexPath) ? "NOT SELECTED" : targetIndexPath);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[4] - Pick target language code: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(targetLanguage == ExHLanguage.Null ? "NOT SELECTED" : targetLanguage.ToString());

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[5] - Back up target file?: ");
            Console.ForegroundColor = backUpTarget ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
            Console.WriteLine(backUpTarget.ToString().ToUpper());

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[6] - Build with above settings.");

            Console.WriteLine();
            Console.Write("Choose an option: ");
        }
    }
}