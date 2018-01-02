using System;
using System.Linq;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Worker.ObjectMappers
{
    public class ActivityMapper : IActivityMapper
    {
        private const string AccountIdPropertyName = "AccountId";
        private const string CreatedAtPropertyName = "CreatedAt";

        public Activity Map<T>(T from, ActivityType to, Func<T, long> accountId = null, Func<T, DateTime> createdAt = null) where T : class
        {
            var data = from.GetType()
                .GetProperties()
                .Select(p => new
                {
                    Key = p.Name,
                    Value = p.GetValue(from)
                })
                .Where(x =>
                {
                    switch (x.Key)
                    {
                        case AccountIdPropertyName:
                            if (accountId == null)
                            {
                                if (x.Value == null)
                                {
                                    throw new Exception($"'{AccountIdPropertyName}' cannot be null.");
                                }

                                accountId = a => (long)x.Value;
                            }

                            return false;
                        case CreatedAtPropertyName:
                            if (createdAt == null)
                            {
                                if (x.Value == null)
                                {
                                    throw new Exception($"'{CreatedAtPropertyName}' cannot be null.");
                                }

                                createdAt = a => (DateTime)x.Value;
                            }

                            return false;
                        default:
                            return true;
                    }
                })
                .ToDictionary(x => x.Key, x => x.Value?.ToString());

            if (accountId == null)
            {
                throw new Exception($"Could not find an '{AccountIdPropertyName}' property.");
            }

            if (createdAt == null)
            {
                throw new Exception($"Could not find a '{CreatedAtPropertyName}' property.");
            }

            return new Activity
            {
                AccountId = accountId(from),
                At =  createdAt(from),
                Created = DateTime.UtcNow,
                Data = data,
                Description = to.GetDescription(),
                Type = to
            };
        }
    }
}