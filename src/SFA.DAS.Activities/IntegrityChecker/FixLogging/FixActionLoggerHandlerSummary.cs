namespace SFA.DAS.Activities.IntegrityChecker.FixLogging
{
    public class FixActionLoggerHandlerSummary
    {
        public string Handler { get; set; }
        public int Occurrences { get; set; }
        public int Success { get; set; }
        public int Fail { get; set; }
        public long ExecutionTime { get; set; }
    }
}