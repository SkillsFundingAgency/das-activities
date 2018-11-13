using CommandLine;

namespace SFA.DAS.Activities.Client.TestHost.CommandLines
{
    [Verb("localizer", HelpText= "Executes the localizers against the activities for the specified accounts")]
    public class LocalizerCommandLineArgs
    {
        [Option('a', "accounts", HelpText = "Account to query (in print page format, e.g. 1-5,8,11)")]
        public string AccountIds { get; set; }
    }
}