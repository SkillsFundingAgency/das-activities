using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Dto
{
    public class ActivityPageResult
    {
        public ActivityPageResult(Activity[] activities, IPagingData pagingData)
        {
            Activities = activities;
            PagingData = pagingData;
        }

        public Activity[] Activities { get; }

        public IPagingData PagingData { get; set; }
    }
}