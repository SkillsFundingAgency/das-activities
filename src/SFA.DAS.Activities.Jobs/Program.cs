using System;
using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using SFA.DAS.Activities.Jobs.DependencyResolution;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Jobs
{
    class Program
    {
        private static readonly ILog Log = new NLogLogger(typeof(Program));

        static void Main()
        {
            try
            {
                Log.Info($"Starting {nameof(JobHostRunner)}");

                JobHostRunner
                    .Create(IoC.InitializeIoC)
                    .RunAndBlock();

                Log.Info($"Stopping {nameof(JobHostRunner)}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{nameof(JobHostRunner)} has faulted");
            }
        }
    }
}
