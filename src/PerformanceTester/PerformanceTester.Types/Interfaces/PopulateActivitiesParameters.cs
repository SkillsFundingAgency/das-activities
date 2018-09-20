namespace PerformanceTester.Types.Interfaces
{
    public class PopulateActivitiesParameters
    {
        public int FirstAccountNumber { get; set; }
        public int NumberOfAccountsRequired { get; set; }

        public int NumberOfActivitgiesPerAccount { get; set; }

        public int NumberOfActivitiesPerDay { get; set; }
    }
}