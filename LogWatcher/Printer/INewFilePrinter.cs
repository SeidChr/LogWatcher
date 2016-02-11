namespace LogWatcher.Printer
{
    interface INewFilePrinter : INamedComponent
    {
        void PrintNewFile(string fileName);
    }
}
