using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Activities.API.Types.DTOs;

namespace SFA.DAS.Activities.API.Client
{
    public interface IActivitiesApiClient
    {
        Task<IEnumerable<ActivityDto>> GetActivities(string ownerId);
    }
}
