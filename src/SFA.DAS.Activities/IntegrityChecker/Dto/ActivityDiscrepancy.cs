namespace SFA.DAS.Activities.IntegrityChecker.Dto
{
    public class ActivityDiscrepancy
    {
        public ActivityDiscrepancy(Activity activity, ActivityDiscrepancyType issues)
        {
            Activity = activity;
            Issues = issues;
        }

        public Activity Activity { get; }
        public ActivityDiscrepancyType Issues { get; }
    }
}