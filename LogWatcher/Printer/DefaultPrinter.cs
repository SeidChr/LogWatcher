using System;
using System.ComponentModel.Composition;
using LogWatcher.Interface;

namespace LogWatcher.Printer
{
    [Export(typeof(ILogLinePrinter))]
    class DefaultPrinter : ILogLinePrinter
    {
        public virtual string Name => "Default";

        protected string lastFile = null;

        public virtual void PrintNewFile(string fileName)
        {
            if (!string.Equals(lastFile, fileName, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Printing changes in File: " + fileName);
                lastFile = fileName;
            }
        }

        public virtual void PrintLogLine(string fileName, string logLine)
        {
            PrintNewFile(fileName);
            Console.WriteLine(logLine);
        }
    }
}
