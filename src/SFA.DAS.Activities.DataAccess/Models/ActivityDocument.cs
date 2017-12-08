using System;
using Nest;
using NuGet;

namespace SFA.DAS.Activities.DataAccess.Models
{
    public class ActivityDocument : Activity
    {
        [Keyword(NullValue = "null")]
        public string SortableTypeOfActivity => TypeOfActivity;

        [Keyword(NullValue = "null")]
        public DateTime SortablePostedDateTime => PostedDateTime;

        [Keyword(NullValue = "null")]
        public DateTime SortablePostedDate => PostedDateTime.Date;

        public ActivityDocument(Activity activity)
        {
            this.AccountId = activity.AccountId;
            this.Data = activity.Data;
            this.PostedDateTime = activity.PostedDateTime;
            this.ProviderUkprn = activity.ProviderUkprn;
            this.TypeOfActivity = activity.TypeOfActivity;
        }
    }
}
