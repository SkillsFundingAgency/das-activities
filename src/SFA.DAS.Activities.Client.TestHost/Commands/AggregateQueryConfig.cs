namespace SFA.DAS.Activities.Client.TestHost.Commands
{
    internal class AggregateQueryConfig
    {
        public string AccountIds { get; set; }

        public string SaveQueryResultsFolder { get; set; }

        public bool IgnoreNotFound { get; set; } = true;
    }
}