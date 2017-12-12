﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using Topshelf;

namespace SFA.DAS.Activities.Worker
{
    public class HostService : ServiceControl
    {
        private readonly IMessageProcessor[] _processors;
        private readonly ILog _log;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private Task _task;

        public HostService(IMessageProcessor[] processors, ILog log)
        {
            _processors = processors;
            _log = log;
        }

        public bool Start(HostControl hostControl)
        {
            _log.Info($"Service starting up");
            _task = Run();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _cancel.Cancel();
            _task.Wait();

            return true;
        }

        async Task Run()
        {
            try
            {
                var tasks = _processors.Select(x => x.RunAsync(_cancel.Token)).ToArray();
                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "Unexpected Exception");
            }
        }
    }
}