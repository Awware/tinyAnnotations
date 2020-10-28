using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tinyAnnotations.ConfigAnalysis.Parser;
using tinyAnnotations.ConfigCreator;
using tinyAnnotations.Lexer;

namespace tinyAnnotations.Tests
{
    public static class CLI
    {
        private const string BaseTitle = "tinyAnnotations.Tests | ";
        private static readonly Version V = new Version(1, 1, 0, 1);
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

            IOConfig cfg = new IOConfig();
            cfg
                .BuildAttribute("A", 1.5, "Test description")
                .BuildAttribute("B", new RandStructure(5, 25), "Random structure");

            tiny t = new tiny(cfg.Generate());

            //string content = File.ReadAllText("tiny.cfg");
            //tiny tiny = new tiny(content);
            $"Value A -> {t["A"]}".ShowAsDebug();
            $"Value B -> {t["B"]}".ShowAsDebug();
            Console.Read();
        }
    }
}
