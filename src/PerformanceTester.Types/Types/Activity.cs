using System;
using System.Collections.Generic;

namespace PerformanceTester.Types
{
    public class Activity : IComparable
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public DateTime At { get; set; }
        public DateTime Created { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string Description { get; set; }
        public ActivityType Type { get; set; }
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }

            if (obj is Activity activity)
            {
                return Id.CompareTo(activity.Id);
            }

            throw new ArgumentException($"Cannot compare type {nameof(Activity)} to {obj.GetType().FullName}");
        }
    }
}
