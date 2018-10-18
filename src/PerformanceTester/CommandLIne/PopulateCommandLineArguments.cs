using CommandLine;

namespace PerformanceTester.CommandLIne
{
    [Verb("populate", HelpText = "Populate enabled stores with the specified number of activities")]
    public class PopulateCommandLineArguments : StoreFilteringCommandLine
    {
        [Option('n', "numberofaccounts", HelpText = "The number of accounts to create", Required = false)]
        public int NumberOfAccounts { get; set; }

        [Option('a', "activitiesperaccount", HelpText = "The number of activities to create per account", Required = false)]
        public int NumberOfActivitiesPerAccount { get; set; }

        [Option('d', "activitiesperday", HelpText = "The number of activities to create per day per account", Required = false)]
        public int NumberOfActivitiesPerDay { get; set; }
    }
}