using System;

namespace SFA.DAS.Activities.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LocalizerAttribute : Attribute
    {
        public Type Type { get; }

        public LocalizerAttribute(Type type)
        {
            Type = type;
        }
    }
}