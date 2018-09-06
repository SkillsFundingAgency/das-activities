using System;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    public class AttributeIsNotAWebJobTrigger : Exception
    {
        public AttributeIsNotAWebJobTrigger(Type attributeType) : base($"The attribute {attributeType.FullName} is not a type of web job trigger")
        {
            // just call base
        }
    }
}