using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AllaganTestNode
{
    class Program
    {
        private static string sourceIndexPath = string.Empty;
        private static ExHLanguage sourceLanguage = ExHLanguage.Null;
        private static IndexFile sourceIndex = new IndexFile();

        private static string targetIndexPath = string.Empty;
        private static ExHLanguage targetLanguage = ExHLanguage.Null;
        private static IndexFile targetIndex = new IndexFile();

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
                            PickLanguageCode(sourceIndexPath, ref sourceLanguage, sourceIndex);
                            break;
                        case 2:
                            PickPath(ref targetIndexPath, "WRITE to");
                            PickLanguageCode(targetIndexPath, ref targetLanguage, targetIndex);
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Input is invalid. Press ENTER to continue...");
                    Console.ReadLine();
                }
            }
        }

        private static string lastLine;
        public static void Report(string line)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            if (!string.IsNullOrEmpty(lastLine))
            {
                string cleanLine = string.Empty;
                while (cleanLine.Length != lastLine.Length) cleanLine += " ";
                Console.Write(cleanLine + "\r");
            }

            Console.Write(line + "\r");
            lastLine = line;
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
            Console.WriteLine("[1] - Pick source file and language code");
            Console.Write("-> ");
            isBuildable &= !string.IsNullOrEmpty(sourceIndexPath);
            Console.ForegroundColor = string.IsNullOrEmpty(sourceIndexPath) ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(string.IsNullOrEmpty(sourceIndexPath) ? "FILE NOT SELECTED" : sourceIndexPath);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("-> ");
            isBuildable &= sourceLanguage != ExHLanguage.Null;
            Console.ForegroundColor = sourceLanguage == ExHLanguage.Null ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(sourceLanguage == ExHLanguage.Null ? "LANG NOT SELECTED" : sourceLanguage.ToString());
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[2] - Pick target file and language code");
            Console.Write("-> ");
            isBuildable &= !string.IsNullOrEmpty(targetIndexPath);
            Console.ForegroundColor = string.IsNullOrEmpty(targetIndexPath) ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(string.IsNullOrEmpty(targetIndexPath) ? "FILE NOT SELECTED" : targetIndexPath);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("-> ");
            isBuildable &= targetLanguage != ExHLanguage.Null;
            Console.ForegroundColor = targetLanguage == ExHLanguage.Null ? ConsoleColor.DarkRed : ConsoleColor.DarkGreen;
            Console.WriteLine(targetLanguage == ExHLanguage.Null ? "LANG NOT SELECTED" : targetLanguage.ToString());
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[3] - Back up target file?");
            Console.Write("-> ");
            Console.ForegroundColor = backUpTarget ? ConsoleColor.DarkGreen : ConsoleColor.Gray;
            Console.WriteLine(backUpTarget ? "YES" : "NO");
            Console.WriteLine();

            Console.ForegroundColor = isBuildable ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
            Console.WriteLine("[4] - Build with above settings.");
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

        static void PickLanguageCode(string path, ref ExHLanguage language, IndexFile indexFile)
        {
            try
            {
                Console.Clear();
                Console.WriteLine();

                if (string.IsNullOrEmpty(path) || !File.Exists(path)) throw new Exception("Specified index file path is incorrect. Please make sure you selected a file first!");
                
                Dictionary<ExHLanguage, bool> availableLanguages = indexFile.Load(path);
                ExHLanguage[] availableLanguagesArray = availableLanguages.Keys.ToArray();

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Gray;

                    for (int i = 0; i < availableLanguagesArray.Length; i++)
                    {
                        Console.WriteLine(string.Format("[{0}] - {1}", i.ToString(), availableLanguagesArray[i].ToString()));
                    }

                    Console.Write("Pick the language code: ");
                    string input = Console.ReadLine();

                    if (!int.TryParse(input, out int _input)) continue;
                    if (_input < 0 || _input >= availableLanguagesArray.Length) continue;

                    language = availableLanguagesArray[_input];

                    return;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e.Message);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Press ENTER to go back to the main menu...");
                Console.ReadLine();

                return;
            }
        }
    }
}