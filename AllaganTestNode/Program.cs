using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AllaganTestNode
{
    class Program
    {
        private static string sourceIndexPath = string.Empty;
        private static ExHLanguage sourceLanguage = ExHLanguage.Null;

        private static string targetIndexPath = string.Empty;
        private static ExHLanguage targetLanguage = ExHLanguage.Null;

        private static bool backUpTarget = true;

        [STAThread]
        static void Main(string[] args)
        {
            while (true)
            {
                PrintScreen();

                string input = Console.ReadLine();
                Console.WriteLine();

                if (int.TryParse(input, out int _input) && _input > 0 && _input <= 6)
                {
                    switch (_input)
                    {
                        case 1:
                            PickPath(ref sourceIndexPath, "READ from");
                            break;
                        case 2:
                            PickLanguageCode(sourceIndexPath, ref sourceLanguage);
                            break;
                        case 3:
                            PickPath(ref targetIndexPath, "WRITE to");
                            break;
                        case 4:
                            PickLanguageCode(targetIndexPath, ref targetLanguage);
                            break;
                        case 5:
                            break;
                        case 6:
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Input is invalid. Press ENTER to continue.");
                    Console.ReadLine();
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
            Console.WriteLine("Pick an operation to execute: ");
            Console.WriteLine();

            bool isBuildable = true;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[1] - Pick source file");
            Console.Write("-> ");
            isBuildable &= !string.IsNullOrEmpty(sourceIndexPath);
            Console.ForegroundColor = string.IsNullOrEmpty(sourceIndexPath) ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(string.IsNullOrEmpty(sourceIndexPath) ? "NOT SELECTED" : sourceIndexPath);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[2] - Pick source language code");
            Console.Write("-> ");
            isBuildable &= sourceLanguage != ExHLanguage.Null;
            Console.ForegroundColor = sourceLanguage == ExHLanguage.Null ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(sourceLanguage == ExHLanguage.Null ? "NOT SELECTED" : sourceLanguage.ToString());
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[3] - Pick target file");
            Console.Write("-> ");
            isBuildable &= !string.IsNullOrEmpty(targetIndexPath);
            Console.ForegroundColor = string.IsNullOrEmpty(targetIndexPath) ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(string.IsNullOrEmpty(targetIndexPath) ? "NOT SELECTED" : targetIndexPath);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[4] - Pick target language code");
            Console.Write("-> ");
            isBuildable &= targetLanguage != ExHLanguage.Null;
            Console.ForegroundColor = targetLanguage == ExHLanguage.Null ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(targetLanguage == ExHLanguage.Null ? "NOT SELECTED" : targetLanguage.ToString());
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[5] - Back up target file?");
            Console.Write("-> ");
            Console.ForegroundColor = backUpTarget ? ConsoleColor.DarkGreen : ConsoleColor.Gray;
            Console.WriteLine(backUpTarget ? "YES" : "NO");
            Console.WriteLine();

            Console.ForegroundColor = isBuildable ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
            Console.WriteLine("[6] - Build with above settings.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Choose an option: ");
        }

        static void PickPath(ref string path, string operation)
        {
            Console.Clear();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(string.Format("Please choose the 0a0000.win32.index file that you will {0}.", operation));
            Console.WriteLine("The file can usually be found under FFXIV\\game\\sqpack\\ffxiv");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press ENTER to open the file picker...");
            Console.ReadLine();

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(path);
                }

                ofd.Filter = "0a0000 Index file (0a0000.win32.index)|0a0000.win32.index";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    path = ofd.FileName;
                }
            }
        }

        static void PickLanguageCode(string path, ref ExHLanguage language)
        {
            Console.Clear();
            Console.WriteLine();

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Specified index file path is incorrect. Please make sure you selected a file first!");
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Press ENTER to go back to the main menu...");
                Console.ReadLine();

                return;
            }
        }
    }
}