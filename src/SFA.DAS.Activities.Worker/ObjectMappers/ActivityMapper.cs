using System;
using System.Linq;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Worker.ObjectMappers
{
    public class ActivityMapper : IActivityMapper
    {
        public Activity Map<T>(T from, ActivityType to, Func<T, long> accountId = null, Func<T, DateTime> at = null) where T : class
        {
            var data = from.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(from)?.ToString());

            return new Activity
            {
                AccountId = accountId?.Invoke(from) ?? long.Parse(data["AccountId"]),
                At =  at?.Invoke(from) ?? DateTime.Parse(data["CreatedAt"]),
                Created = DateTime.UtcNow,
                Data = data,
                Description = to.GetDescription(),
                Type = to
            };
        }
    }
}