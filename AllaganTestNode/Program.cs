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
                Console.ReadLine();
            }
        }

        static void PrintScreen()
        {
            Console.Clear();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(string.Format("Allagan Test Node v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("Pick an operation to execute:");
            Console.WriteLine(string.Format("[1] - Pick source file: {0}", string.IsNullOrEmpty(sourceIndexPath) ? "NOT SELECTED" : sourceIndexPath));
            Console.WriteLine(string.Format("[2] - Pick source language code: {0}", sourceLanguage == ExHLanguage.Null ? "NOT SELECTED" : sourceLanguage.ToString()));
            Console.WriteLine(string.Format("[3] - Pick target file: {0}", string.IsNullOrEmpty(targetIndexPath) ? "NOT SELECTED" : targetIndexPath));
            Console.WriteLine(string.Format("[4] - Pick target language code: {0}", targetLanguage == ExHLanguage.Null ? "NOT SELECTED" : targetLanguage.ToString()));
            Console.WriteLine(string.Format("[5] - Back up target file?: {0}", backUpTarget.ToString()));


        }
    }
}