namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public class ActivityPageResult
    {
        public ActivityPageResult(Activity[] activities, bool atEnd)
        {
            Activities = activities;
            AtEnd = atEnd;
        }

        public Activity[] Activities { get; }
        public bool AtEnd { get; set; }

    }
}