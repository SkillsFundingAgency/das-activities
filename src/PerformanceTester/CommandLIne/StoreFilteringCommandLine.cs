using System.Collections.Generic;
using CommandLine;

namespace PerformanceTester.CommandLIne
{
    public class StoreFilteringCommandLine
    {
        [Option('s', "store", HelpText = "List of stores that are enabled - default is all", Required = false)]
        public IEnumerable<string> EnableStore { get; set; }
    }
}