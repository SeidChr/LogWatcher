using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using LogWatcher.Interface;

namespace LogWatcher.Printer
{
    [Export(typeof(ILogLinePrinter))]
    class ColorPrinter : DefaultPrinter
    {
        public override string Name { get; } = "Color";

        private ConsoleColor lastServerityColor = Console.ForegroundColor;

        public override void PrintLogLine(string fileName, string logLine)
        {
            PrintNewFile(fileName);

            var oldColor = Console.ForegroundColor;
            switch (GetLogSeverity(logLine).ToLowerInvariant())
            {
                case "error":
                    Console.ForegroundColor = ConsoleColor.Red;
                    
                    break;
                case "warn":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "info":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.ForegroundColor = lastServerityColor;
                    break;
            }

            lastServerityColor = Console.ForegroundColor;

            Console.WriteLine(logLine);

            Console.ForegroundColor = oldColor;
        }

        private string GetLogSeverity(string logLine)
        {
            var result = "";
            var loweredLogLine = logLine.ToLowerInvariant();

            if (Regex.IsMatch(loweredLogLine, "\\serror\\s"))
            {
                result = "error";
            }
            else if (Regex.IsMatch(loweredLogLine, "\\swarn\\s"))
            {
                result = "warn";
            }
            else if (Regex.IsMatch(loweredLogLine, "\\sinfo\\s"))
            {
                result = "info";
            }

            return result;
        }
    }
}
