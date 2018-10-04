using CommandLine;

namespace SFA.DAS.Activities.Client.TestHost.CommandLines
{
    [Verb("query", HelpText=",")]
    public class QueryCommandLineArgs
    {
        [Option('a', "account", HelpText = "An account to query")]
        public int AccountId { get; set; }

        [Option('t', "type", HelpText = "Type of query to run")]
        public QueryType QueryType { get; set; }
    }
}