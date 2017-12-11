using System;
using System.Collections.Generic;
using Nest;
using NuGet;

namespace SFA.DAS.Activities.DataAccess.Models
{
    public class ActivityDocument
    {
        [Keyword(NullValue = "null")]
        public string TypeDesc { get; set; }

        public ActivityTypeEnum Type { get; set; }

        [Date]
        public DateTime At { get; set; }

        [Keyword(NullValue = "null")]
        public long? AccountId { get; set; }

        [Keyword(NullValue = "null")]
        public long? ProviderUkprn { get; set; }

        [Object]
        public IDictionary<string, string> Data { get; set; }

        public string Creator { get; set; }
        public string CreatorUserRef { get; set; }
    }
}
