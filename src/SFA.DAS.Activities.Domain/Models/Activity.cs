using NuGetProject.Enums;
using System;


namespace SFA.DAS.Activities.Domain.Models
{
    public class Activity
    {
        public Guid Id { get; set; }
        public ActivityType Type { get; set; }
        public string AccountId { get; set; }
        public string Description { get; set; }

        public string Url { get; set; }
    }
}
