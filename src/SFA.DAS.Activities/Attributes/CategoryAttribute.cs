using System;

namespace SFA.DAS.Activities.Attributes
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