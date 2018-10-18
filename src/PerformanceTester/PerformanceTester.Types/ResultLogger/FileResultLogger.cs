using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;
using System;
using System.IO;
using System.Text;
using PerformanceTester.Types.Parameters;

namespace PerformanceTester.Types.ResultLogger
{
    public class FileResultLogger : IResultLogger, IDisposable
    {
        private FileStream _fileStream;
        private readonly Lazy<StreamWriter> _streamWriter;

        public FileResultLogger(IConfigProvider configProvider)
        {
            _streamWriter = new Lazy<StreamWriter>(() => OpenFile(configProvider));
        }

        public void LogStore(StoreTaskDetails storeDetail)
        {
            // Nothing to do here - we're writing a single line type to make it easier to work with the data
        }

        public void LogCost(StoreTaskDetails storeDetail, IOperationCost operationCost, int groupLevel)
        {
            EnsureHeadings();

            WriteLine(
                storeDetail.Store.Name,
                storeDetail.Command.GetType().Name,
                $"{storeDetail.Elapsed.TotalSeconds:F2}",
                storeDetail.Success ? "Successful" : "Failed", 
                groupLevel.ToString(), 
                operationCost.Operation, 
                $"{operationCost.Cost:F2}", 
                $"{operationCost.ElapsedMSecs:F2}");
        }

        private bool _headersWritten = false;

        private void EnsureHeadings()
        {
            if (_headersWritten)
            {
                return;
            }

            WriteLine(
                "Data-store",
                "Command",
                "Command-Elapsed-MSecs",
                "Result",
                "Nested-Level",
                "Operation-Name",
                "Operation-Cost",
                "Operation-Elapsed-MSecs");

            _headersWritten = true;
        }

        private void WriteLine(params string[] args)
        {
            var sw = _streamWriter.Value;

            for(var i = 0; i < args.Length; i++)
            {
                sw.Write(args[i]);
                if (i < args.Length - 1)
                {
                    sw.Write(',');
                }
            }

            sw.Write(Environment.NewLine);
        }

        private StreamWriter OpenFile(IConfigProvider configProvider)
        {
            var parameters = configProvider.Get<LogFileParameters>();
            var logFileName = Path.Combine(parameters.LogFolder, $"PerformanceTester_{DateTime.Now:yyyy-MM-dd_hhmmss}.log");
            try
            {
                var fs = new FileStream(logFileName, FileMode.CreateNew);
                var ts = new StreamWriter(fs, Encoding.UTF8);
                _fileStream = fs;
                Console.WriteLine($"Log file :\"{logFileName}\"");
                return ts;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not open log file \"{logFileName}\" - {e.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            if (_streamWriter.IsValueCreated)
            {
                _streamWriter.Value.Close();
                _streamWriter.Value.Dispose();
                _fileStream.Close();
                _fileStream.Dispose();
            }
        }
    }
}
