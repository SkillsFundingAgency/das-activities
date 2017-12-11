using System;
using System.Collections.Generic;
using Nest;
using NuGet;

namespace SFA.DAS.Activities.DataAccess.Models
{
    public class ActivityDocument : Activity
    {
        [Keyword(NullValue = "null")]
        public string TypeDesc => Type.ToString();

        [Date]
        public sealed override DateTime At { get; set; }

        [Keyword(NullValue = "null")]
        public sealed override long? AccountId { get; set; }

        [Keyword(NullValue = "null")]
        public sealed override long? ProviderUkprn { get; set; }

        [Object]
        public sealed override IDictionary<string, string> Data { get; set; }


        public ActivityDocument(Activity activity)
        {

            this.AccountId = activity.AccountId;
            this.Data = activity.Data;
            this.At = activity.At;
            this.ProviderUkprn = activity.ProviderUkprn;
            this.Type = activity.Type;
            this.CreatedBy = activity.CreatedBy;
        }
    }
}
