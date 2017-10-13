﻿

using MediatR;

namespace SFA.DAS.Activities.Application.Queries
{
    public class GetActivitiesByOwnerIdRequest : IAsyncRequest<GetActivitiesByOwnerIdResponse>
    {
        public GetActivitiesByOwnerIdRequest(string ownerId)
        {
            OwnerId = ownerId;
        }
        public string OwnerId { get;}
    }
}