using System;
using System.ComponentModel.Composition;

namespace LogWatcher.Printer
{
    [Export(typeof(ILogLinePrinter))]
    [Export(typeof(INewFilePrinter))]
    class ColorPrinter : DefaultPrinter
    {
        public override string Name { get; } = "Color";

        public override void PrintLogLine(string logLine)
        {
            var oldColor = Console.ForegroundColor;
            switch (GetLogSeverity(logLine, 2).ToLowerInvariant())
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
            }

            printedLogLine = true;
            Console.WriteLine(logLine);

            Console.ForegroundColor = oldColor;
        }

        private string GetLogSeverity(string logLine, int index)
        {
            var result = "";
            var lineSplit = logLine.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (lineSplit.Length >= index+1)
            {
                result = lineSplit[index];
            }

            return result;
        }
    }
}
