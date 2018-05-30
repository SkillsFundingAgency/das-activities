namespace SFA.DAS.Activities.IntegrityChecker.Dto
{
    public class ActivityDiscrepancy
    {
        public ActivityDiscrepancy(Activity id, ActivityDiscrepancyType issues)
        {
            Id = id;
            Issues = issues;
        }

        public Activity Id { get; }
        public ActivityDiscrepancyType Issues { get; }
    }
}