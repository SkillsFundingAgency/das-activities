namespace PerformanceTester.Types.Parameters
{
    public class PopulateActivitiesParameters
    {
        public int FirstAccountNumber { get; set; }
        public int NumberOfAccountsRequired { get; set; }

        public int NumberOfActivitiesPerAccount { get; set; }

        public int NumberOfActivitiesPerDay { get; set; }
    }
}