using System;
using System.Linq;
using SFA.DAS.Activities.Extensions;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.Worker.ObjectMappers
{
    public class ActivityMapper : IActivityMapper
    {
        public Activity Map<T>(T from, ActivityType to, Func<T, long> accountId = null, Func<T, DateTime> createdAt = null, Guid? messageId = null) where T : class
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
                        case nameof(AccountMessageBase.AccountId):
                            if (accountId == null)
                            {
                                if (x.Value == null)
                                {
                                    throw new Exception($"'{nameof(AccountMessageBase.AccountId)}' cannot be null.");
                                }

                                accountId = a => (long)x.Value;
                            }

                            return false;
                        case nameof(AccountMessageBase.CreatedAt):
                            if (createdAt == null)
                            {
                                if (x.Value == null)
                                {
                                    throw new Exception($"'{nameof(AccountMessageBase.CreatedAt)}' cannot be null.");
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
                throw new Exception($"Could not find an '{nameof(AccountMessageBase.AccountId)}' property.");
            }

            if (createdAt == null)
            {
                throw new Exception($"Could not find a '{nameof(AccountMessageBase.CreatedAt)}' property.");
            }

            return new Activity
            {
                Id = messageId ?? Guid.Empty,
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