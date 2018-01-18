﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using Topshelf;

namespace SFA.DAS.Activities.Worker
{
    public class Service : ServiceControl
    {
        private readonly IEnumerable<IMessageProcessor> _processors;
        private readonly ILog _log;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private Task _task;

        public Service(IEnumerable<IMessageProcessor> processors, ILog log)
        {
            _processors = processors;
            _log = log;
        }

        public bool Start(HostControl hostControl)
        {
            _log.Info("Service starting.");
            _task = Run();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _log.Info("Service stopping.");
            _cancel.Cancel();
            _task.Wait();

            return true;
        }

        private async Task Run()
        {
            await Task.WhenAll(_processors.Select(p => p.RunAsync(_cancel.Token)).ToArray());
        }
    }
}