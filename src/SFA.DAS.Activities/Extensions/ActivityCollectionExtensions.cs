using System;

namespace SFA.DAS.Activities.Extensions
{
    internal static class ActivityCollectionExtensions
    {
        public static void AssertInIdOrder(this Activity[] activities)
        {
            for (int i = 1; i < activities.Length; i++)
            {
                if (activities[i - 1].Id.CompareTo(activities[i].Id) >= 0)
                {
                    throw new Exception("Messages are not in the correct sequence");
                }
            }
        }
    }
}