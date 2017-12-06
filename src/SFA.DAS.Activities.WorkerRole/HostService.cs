using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using Topshelf;
using Topshelf.Logging;

namespace ESFA.DAS.Support.Indexer.Worker
{
    public class HostService : ServiceControl
    {
        private readonly IMessageProcessor[] _processors;
        readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        Task _task;
        private ILog _log;

        public HostService(IMessageProcessor[] processors, ILog log)
        {
            _processors = processors;
            _log = log;
        }

        public bool Start(HostControl hostControl)
        {
            _log.Info($"Service starting up");

            _task = Idle();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _cancel.Cancel();

            _task.Wait();

            return true;
        }

        async Task Idle()
        {
            if (_cancel.Token.IsCancellationRequested)
            {
                _log.Info("Service stopping");
                return;
            }

            await Task.Yield();

            try
            {
                var tasks = _processors.Select(x => x.RunAsync(_cancel.Token)).ToArray();
                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "Unexpected Exception");
            }


            if (Debugger.IsAttached)
            {
                _cancel.Cancel();
            }

            await Task.Delay(1000);
            await Idle();
        }
    }
}