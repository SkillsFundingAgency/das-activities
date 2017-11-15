using System;
using System.Collections.Generic;
using Nest;

namespace NuGet
{
    public class Activity
    { 
        public string TypeOfActivity { get; internal set; }

        [Keyword(NullValue = "null")]
        public string TypeOfActivityKeyword => TypeOfActivity;

        public string DescriptionOne { get; internal set; }

        public string DescriptionTwo { get; internal set; }

        /// <summary>
        /// The description to be used  in for instance '1 thing happened'. The value here should be 'thing happened'
        /// </summary>
        public string DescriptionSingular{ get; internal set; }

        /// <summary>
        /// The description to be used  in for instance '2 things happened'. The value here should be 'things happened'
        /// </summary>
        public string DescriptionPlural { get; internal set; }

        public List<string> AssociatedData { get; internal set; } =new List<string>();

        public string Url { get; internal set; }

        public DateTime PostedDateTime { get; internal set; }

        [Keyword(NullValue = "null")]
        public DateTime PostedDateTimeKeyword => PostedDateTime;

        [Keyword(NullValue = "null")]
        public DateTime PostedDateKeyword => PostedDateTime.Date;

        public string AccountId { get; internal set; }


        public static class ActivityTypeStrings
        {
            public const string UserJoined = "UserJoined";
            public const string UserInvited = "UserInvited";
            public const string AccountCreated = "AccountCreated";
            public const string PayeSchemeCreatedy = "PayeSchemeCreated";
            public const string PayeSchemeDeleted = "PayeSchemeDeleted";
            public const string AgreementCreated = "AgreementCreated";
            public const string AgreementSigned = "AgreementSigned";
            public const string AccountNameChanged = "AccountNameChanged";
            public const string LevyPaymentRecieved = "LevyPaymentRecieved";
        }
    }
}
