namespace LogWatcher.Printer
{
    public interface ILogLinePrinter : INamedComponent
    {
        void PrintLogLine(string logLine);
    }
}
