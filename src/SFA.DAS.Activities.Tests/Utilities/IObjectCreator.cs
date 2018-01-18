using System;

namespace SFA.DAS.Activities.Tests.Utilities
{
    public interface IObjectCreator
    {
        object Create(Type type, object properties = null);
    }
}