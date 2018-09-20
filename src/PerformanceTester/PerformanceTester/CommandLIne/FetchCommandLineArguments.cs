using System.Collections.Generic;
using CommandLine;

namespace PerformanceTester.CommandLIne
{
    [Verb("fetch", HelpText = "Fetch a series of activities from each store")]
    public class FetchCommandLineArguments : StoreFilteringCommandLine
    {
        [Option('a', "accounts", HelpText = "List of accounts to fetch (in 'print pages' format). Default will be picked up from config", Required = false)]
        public string AccountIds { get; set; }
    }
}