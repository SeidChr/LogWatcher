using System;
using System.ComponentModel.Composition;

namespace LogWatcher.Printer
{
    [Export(typeof(ILogLinePrinter))]
    [Export(typeof(INewFilePrinter))]
    class DefaultPrinter : ILogLinePrinter, INewFilePrinter
    {
        public virtual string Name => "Default";

        protected bool printedLogLine;

        public virtual void PrintNewFile(string fileName)
        {
            if (printedLogLine)
            {
                Console.WriteLine();
                Console.WriteLine();
                printedLogLine = false;
            }

            Console.WriteLine("Printing changes in File: " + fileName);
        }

        public virtual void PrintLogLine(string logLine)
        {
            printedLogLine = true;
            Console.WriteLine(logLine);
        }
    }
}
