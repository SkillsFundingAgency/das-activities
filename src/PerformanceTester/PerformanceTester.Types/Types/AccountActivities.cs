using System.Collections.Generic;

namespace PerformanceTester.Types
{
    public class AccountActivities
    {
        public long AccountId { get; set; }
        public List<Activity> Activites { get; set; }
    }
}