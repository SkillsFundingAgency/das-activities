using CommandLine;

namespace SFA.DAS.Activities.Client.TestHost.CommandLines
{
    [Verb("query", HelpText= "Executes the GetLatestActivities method on the activities client")]
    public class QueryCommandLineArgs
    {
        [Option('a', "accounts", HelpText = "Account to query (in print page format, e.g. 1-5,8,11)")]
        public string AccountIds { get; set; }


        [Option('i', "ignoreAccountsNotFound", HelpText = "Do not report acccount ids that do not have any activities", Default = true)]
        public bool IgnoreNotFound { get; set; }

        [Option('t', "type", HelpText = "Type of query to run")]
        public QueryType QueryType { get; set; }
    }
}