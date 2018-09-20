using CommandLine;

namespace PerformanceTester.CommandLIne
{
    [Verb("populate", HelpText = "Populate enabled stores with the specified number of activities")]
    public class PopulateCommandLineArguments : StoreFilteringCommandLine
    {
    }
}