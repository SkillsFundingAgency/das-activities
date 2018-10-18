using System.Collections.Generic;
using CommandLine;

namespace PerformanceTester.CommandLIne
{
    [Verb("aggregate", HelpText = "Run an aggregate query")]
    public class AggregateCommandLineArguments : StoreFilteringCommandLine
    {
        [Option('a', "accounts", HelpText = "List of accounts to fetch (in 'print pages' format). Default will be picked up from config", Required = false)]
        public string AccountIds { get; set; }
    }
}