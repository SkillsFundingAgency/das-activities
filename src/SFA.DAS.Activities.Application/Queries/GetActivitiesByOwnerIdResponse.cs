using System;
using System.Collections.Generic;
using SFA.DAS.Activities.Domain.Models;

namespace SFA.DAS.Activities.Application.Queries
{
    public class GetActivitiesByOwnerIdResponse
    {
        public IEnumerable<Activity> Activities { get; set; }
    }
}
