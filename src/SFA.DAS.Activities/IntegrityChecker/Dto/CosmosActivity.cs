using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Activities.IntegrityChecker.Dto
{
    /// <remarks>
    ///     When serialising objects DocumentDb will respect the json serialisation settings, which is what converts the property "Id" to the
    ///     "id" property required by documentdb.
    ///     However, LINQ driver does not uses the serialisation settings - instead it uses the type property directly, only looking for a
    ///     JSONProperty attribute on the property. Consequently a LINQ expression of activity.Id == 223 will actually look for a property
    ///     named "Id", not the one stored "id".
    ///     
    ///     See https://github.com/Azure/azure-documentdb-dotnet/issues/317
    /// 
    ///     This class will map between Activity and CosmosProperty, the latter having the appropriate JSONProperty attributes to allow the 
    ///     LINQ to work.
    /// </remarks>>
    internal class CosmosActivity
    {
 
        public CosmosActivity(Activity activity)
        {
            Activity = activity;
        }

        public CosmosActivity()
        {
            Activity = new Activity();
        }

        [JsonIgnore]
        public Activity Activity { get; }

        [JsonProperty("id")]
        public Guid Id
        {
            get => Activity.Id;
            set => Activity.Id = value;
        }

        /// <summary>
        ///		We cannot order by Id in Cosmos, so have to store separately so that a range index is created over it.
        /// </summary>
        [JsonProperty("messageId")]
        public Guid MessageId => Activity.Id;

        [JsonProperty("accountId")]
        public long AccountId
        {
            get => Activity.AccountId;
            set => Activity.AccountId = value;
        }

        [JsonProperty("at")]
        public DateTime At
        {
            get => Activity.At;
            set => Activity.At = value;
        }

        [JsonProperty("created")]
        public DateTime Created
        {
            get => Activity.Created;
            set => Activity.Created = value;
        }

        [JsonProperty("data")]
        public Dictionary<string, string> Data
        {
            get => Activity.Data;
            set => Activity.Data = value;
        }

        [JsonProperty("description")]
        public string Description
        {
            get => Activity.Description;
            set => Activity.Description = value;
        }

        [JsonProperty("type")]
        public ActivityType Type
        {
            get => Activity.Type;
            set => Activity.Type = value;
        }
    }
}