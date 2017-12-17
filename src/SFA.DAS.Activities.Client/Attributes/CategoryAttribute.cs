using System;

namespace SFA.DAS.Activities.Client.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CategoryAttribute : Attribute
    {
        public ActivityTypeCategory Category { get; }

        public CategoryAttribute(ActivityTypeCategory category)
        {
            Category = category;
        }
    }
}