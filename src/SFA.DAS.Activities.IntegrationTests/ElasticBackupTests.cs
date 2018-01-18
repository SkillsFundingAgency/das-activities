using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Activities.Tests.Utilities;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using StructureMap;

namespace SFA.DAS.Activities.IntegrationTests
{
    public class ElasticBackupTests
    {

        private static readonly List<int> AccountIds = Enumerable.Range(1, 20).ToList();
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
                .ToList();

            await Task.WhenAll(activitiesFromMemory.Select(a => _client.IndexAsync(a, i => i.Refresh(Refresh.True))));

            var activitiesFromElastic = await GetActivitiesFromElastic();

            Assert.That(activitiesFromElastic.Count, Is.EqualTo(activitiesFromMemory.Count));

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(activitiesFromElastic));
        }

        [Test]
        [Ignore("Only intended to be run manually.")]
        public async Task AfterBackup()
        {
            var activitiesFromMemory = JsonConvert.DeserializeObject<List<Activity>>(File.ReadAllText(FilePath));
            var activitiesFromElastic = await GetActivitiesFromElastic();

            Assert.That(activitiesFromElastic.Count, Is.EqualTo(activitiesFromMemory.Count));

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
                    )
                 )
                .ToArray());

            return result.SelectMany(r => r.Documents).ToList();
        }
    }
}