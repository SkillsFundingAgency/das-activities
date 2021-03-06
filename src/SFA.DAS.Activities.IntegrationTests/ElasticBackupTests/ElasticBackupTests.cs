﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers.ObjectMappers;
using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.EmployerAccounts.Events.Messages;
using StructureMap;

namespace SFA.DAS.Activities.IntegrationTests
{
    public class ElasticBackupTests
    {
        private static readonly List<int> AccountIds = Enumerable.Range(1, 50).ToList();
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "activities.json");

        private static readonly Dictionary<Type, ActivityType> Types = new Dictionary<Type, ActivityType>
        {
            [typeof(AccountCreatedMessage)] = ActivityType.AccountCreated,
            [typeof(LegalEntityAddedMessage)] = ActivityType.LegalEntityAdded,
            [typeof(AgreementSignedMessage)] = ActivityType.AgreementSigned,
            [typeof(PayeSchemeAddedMessage)] = ActivityType.PayeSchemeAdded,
            [typeof(AccountNameChangedMessage)] = ActivityType.AccountNameChanged,
            [typeof(UserInvitedMessage)] = ActivityType.UserInvited,
            [typeof(UserJoinedMessage)] = ActivityType.UserJoined,
            [typeof(PayeSchemeDeletedMessage)] = ActivityType.PayeSchemeRemoved,
            [typeof(LegalEntityRemovedMessage)] = ActivityType.LegalEntityRemoved
        };

        private IContainer _container;
        private IActivityMapper _activityMapper;
        private IElasticClient _client;
        private IObjectCreator _objectCreator;

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            _container = new Container(c =>
            {
                c.AddRegistry<ActivitiesIntegrationTestRegistry>();
            });

            _activityMapper = _container.GetInstance<IActivityMapper>();
            _client = _container.GetInstance<IElasticClient>();
            _objectCreator = _container.GetInstance<IObjectCreator>();
        }

        [Test]
        [Ignore("Only intended to be run manually.")]
        public async Task BeforeBackup()
        {
            var activitiesFromMemory = AccountIds
                .SelectMany(i => Types
                    .Select(t =>
                    {
                        var message = _objectCreator.Create(t.Key, new { AccountId = i });
                        var activity = _activityMapper.Map(message, t.Value);

                        return activity;
                    })
                )
                .OrderBy(a => a.AccountId)
                .ThenBy(a => a.Type)
                .ToList();

            await Task.WhenAll(activitiesFromMemory.Select(a => _client.IndexAsync(a, i => i.Refresh(Refresh.True))));

            var activitiesFromElastic = await GetActivitiesFromElastic();

            Verify(activitiesFromMemory, activitiesFromElastic);

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(activitiesFromElastic));
        }

        [Test]
        [Ignore("Only intended to be run manually.")]
        public async Task AfterBackup()
        {
            var activitiesFromMemory = JsonConvert.DeserializeObject<List<Activity>>(File.ReadAllText(FilePath));
            var activitiesFromElastic = await GetActivitiesFromElastic();

            Verify(activitiesFromMemory, activitiesFromElastic);

            File.Delete(FilePath);
        }

        private async Task<List<Activity>> GetActivitiesFromElastic()
        {
            var result = await Task.WhenAll(AccountIds
                .Select(i => _client
                    .SearchAsync<Activity>(s => s
                        .Query(q => q
                            .Term(t => t
                                .Field(f => f.AccountId)
                                .Value(i)
                            )
                        )
                        .Sort(srt => srt
                            .Ascending(a => a.AccountId)
                            .Ascending(a => a.Type)
                        )
                    )
                 )
                .ToArray());

            return result.SelectMany(r => r.Documents).ToList();
        }

        private void Verify(List<Activity> activitiesFromMemory, List<Activity> activitiesFromElastic)
        {
            Assert.That(activitiesFromElastic.Count, Is.EqualTo(activitiesFromMemory.Count));

            for (var i = 0; i < activitiesFromMemory.Count; i++)
            {
                var memoryObj = activitiesFromMemory[i];
                var elasticObj = activitiesFromElastic[i];
                var memoryObjProps = memoryObj.GetType().GetProperties();
                var elasticObjProps = elasticObj.GetType().GetProperties();

                Assert.That(elasticObjProps.Length, Is.EqualTo(memoryObjProps.Length));

                for (var j = 0; j < memoryObjProps.Length; j++)
                {
                    var memoryObjProp = memoryObjProps[j];
                    var elasticObjProp = elasticObj.GetType().GetProperty(memoryObjProp.Name);

                    Assert.That(elasticObjProp, Is.Not.Null);

                    var memoryObjPropVal = memoryObjProp.GetValue(memoryObj);
                    var elasticObjPropVal = elasticObjProp.GetValue(elasticObj);

                    Assert.That(memoryObjProp.PropertyType, Is.EqualTo(elasticObjProp.PropertyType));

                    if (memoryObjProp.PropertyType.IsValueType || memoryObjProp.PropertyType == typeof(string))
                    {
                        Assert.That(elasticObjPropVal, Is.EqualTo(memoryObjPropVal));
                    }
                    else
                    {
                        var memoryObjPropDict = (Dictionary<string, string>)memoryObjPropVal;
                        var elasticObjPropDict = (Dictionary<string, string>)elasticObjPropVal;
                        var match = elasticObjPropDict.Count == memoryObjPropDict.Count && !elasticObjPropDict.Except(memoryObjPropDict).Any();

                        Assert.That(match, Is.True);
                    }
                }
            }
        }
    }
}