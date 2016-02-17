using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading;
using LogWatcher.Interface;
using LogWatcher.Printer;

namespace LogWatcher
{
    class Program
    {
        //private static string lastFileByFileSystemWatcher;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Parameters: \"<dir>\" \"<filter>\" [<printer>=\"Default\"]");
                return;
            }

            string printerName = "Default";

            var dir = args[0];
            var filter = args[1];
            if (args.Length > 2)
            {
                printerName = args[2];
            }

            var programm = new Program();
            programm.Run(dir, filter, printerName);

        }

        [ImportMany(typeof(ILogLinePrinter))]
        private IEnumerable<Lazy<ILogLinePrinter>> LogLinePrinters { get; set; }

        private ILogLinePrinter linePrinter;

        private void Run(string dir, string filter, string printerName = "Default")
        {
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            linePrinter = LogLinePrinters.First(p => string.Equals(p.Value.Name, printerName, StringComparison.InvariantCultureIgnoreCase)).Value;

            var filePositions = new Dictionary<string, long>();

            //var watcher = new FileSystemWatcher();
            //watcher.Path = dir;
            //watcher.Renamed += (sender, renameArgs) => { lastFileByFileSystemWatcher = renameArgs.FullPath; };
            //watcher.EnableRaisingEvents = true;
            //watcher.Filter = filter;
            //watcher.IncludeSubdirectories = true;

            Console.WriteLine($"Watching {dir} for changes on {filter}");

            var currentFile = FindLastChangedFile(dir, filter);

            while (true)
            {
                if (currentFile == null)
                {
                    Thread.Sleep(100);
                    continue;
                }

                using (StreamReader reader = new StreamReader(
                    new FileStream(
                        currentFile,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite)))
                {
                    //start at the end of the file
                    var currentFilePosition = reader.BaseStream.Length;
                    if (filePositions.ContainsKey(currentFile))
                    {
                        currentFilePosition = filePositions[currentFile];
                    }
                    else
                    {
                        filePositions[currentFile] = currentFilePosition;
                    }


                    string lastFile = currentFile;
                    while (string.Equals(currentFile = FindLastChangedFile(dir, filter), lastFile))
                    {
                        Thread.Sleep(100);

                        //if the file size has not changed, idle
                        if (reader.BaseStream.Length == currentFilePosition)
                        {
                            continue;
                        }

                        //seek to the last max offset
                        reader.BaseStream.Seek(currentFilePosition, SeekOrigin.Begin);

                        //read out of the file until the EOF
                        var line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            PrintNewLogLine(currentFile, line);
                        }

                        //update the last max offset and lastFile
                        filePositions[currentFile] = currentFilePosition = reader.BaseStream.Position;
                        lastFile = currentFile;
                    }
                }
            }
        }

        private static string FindLastChangedFile(string dir, string filter)
            => FindLastChangedFileByLastWriteTime(dir, filter);

        //private static string FindLastChangedFileByFilewatcher(string dir, string filter)
        //{
        //    return lastFileByFileSystemWatcher;
        //}

        private static string FindLastChangedFileByLastWriteTime(string dir, string filter)
        {
            var directory = new DirectoryInfo(dir);

            var lastChangedFile = directory
                .GetFiles(filter, SearchOption.TopDirectoryOnly)
                .Select(f => { f.Refresh(); return f; })
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            return lastChangedFile.FullName;
        }
        
        private void PrintNewLogLine(string fileName, string logLine)
        {
            linePrinter.PrintLogLine(fileName, logLine);
            //printedLogLine = true;
            //Console.WriteLine(logLine);
        }
    }
}
