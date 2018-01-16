using System;

namespace SFA.DAS.Activities.AcceptanceTests.Utilities
{
    public interface IObjectCreator
    {
        object Create(Type type, object properties);
    }
}