using System;

namespace SFA.DAS.Activities.Worker.ObjectMappers
{
    public interface IActivityMapper
    {
        Activity Map<T>(T from, ActivityType to, Func<T, long> accountId = null, Func<T, DateTime> at = null) where T : class;
    }
}