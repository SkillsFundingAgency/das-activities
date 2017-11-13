

using MediatR;

namespace SFA.DAS.Activities.Application.Queries.GetActivities
{
    public class GetActivitiesByOwnerIdRequest : IAsyncRequest<GetActivitiesByOwnerIdResponse>
    {
        public GetActivitiesByOwnerIdRequest(long accountId)
        {
            AccountId = accountId;
        }
        public long AccountId { get;}
    }
}
