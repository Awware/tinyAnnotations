using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyAnnotations.ConfigAnalysis.Parser;
using TinyAnnotations.Lexer;

namespace tinyAnnotations.Tests
{
    public static class CLI
    {
        private const string BaseTitle = "tinyAnnotations.Tests | ";
        private static readonly Version V = new Version(1, 6, 0, 0);
        private static void Write(ConsoleColor color, string msg)
        {
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ResetColor();
        }
        private static void Writeln(ConsoleColor color, string msg) => Write(color, msg + "\n");
        public static void ShowAsDebug(this string msg) => Writeln(ConsoleColor.DarkGray, $"[-] {msg}");
        public static void ShowAsError(this string msg) => Writeln(ConsoleColor.Red, $"[!] {msg}");
        public static void ShowAsInfo(this string msg) => Writeln(ConsoleColor.Cyan, $"[?] {msg}");
        public static void ShowAsWarning(this string msg) => Writeln(ConsoleColor.Yellow, $"[&] {msg}");
        public static void UpdateTitle(string newTitle) => Console.Title = BaseTitle + newTitle;
        public static void DisplayBaseTitle()
        {
            UpdateTitle($"v{V.Major}.{V.Minor}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CLI.DisplayBaseTitle();
            string content = File.ReadAllText("tiny.cfg");
            tiny tiny = new tiny(content);
            $"Value of Test -> {tiny["E"]}".ShowAsDebug();
            Console.Read();
        }
    }
}
