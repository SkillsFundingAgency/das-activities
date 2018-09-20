using PerformanceTester.Types.Types;

namespace PerformanceTester.Types.Interfaces
{
    public interface IResultLogger
    {
        void Log(RunDetails details);
    }
}
