namespace LogWatcher.Interface
{
    public interface ILogLinePrinter : INamedComponent
    {
        void PrintLogLine(string fileName, string logLine);
    }
}
