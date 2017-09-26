using System;
using SFA.DAS.Activities.API.Types.Enums;

namespace SFA.DAS.Activities.Domain.Models
{
    public class Activity
    {
        public Guid Id { get; set; }
        public ActivityType Type { get; set; }
        public string OwnerId { get; set; }
        public string Description { get; set; }
    }
}
