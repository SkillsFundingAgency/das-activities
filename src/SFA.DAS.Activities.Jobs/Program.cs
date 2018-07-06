using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using SFA.DAS.Activities.Jobs.DependencyResolution;

namespace SFA.DAS.Activities.Jobs
{
    class Program
    {
        static void Main()
        {
            JobHostRunner
                .Create(IoC.InitializeIoC)
                .RunAndBlock();
        }
    }
}
