using System;
using System.Collections.Generic;
using NuGet;

namespace SFA.DAS.Activities.Application.Queries
{
    public class GetActivitiesByOwnerIdResponse
    {
        public IEnumerable<Activity> Activities { get; set; }
    }
}
