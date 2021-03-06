﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using BoDi;
using NFluent;
using Polly;
using SFA.DAS.Activities.AcceptanceTests.Azure;
using SFA.DAS.Activities.AcceptanceTests.Models;
using SFA.DAS.Activities.Client;
using SFA.DAS.ApiSubstitute.Utilities;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    [Binding]
    public class ActivitiesSteps
    {
        private readonly IEnumerable<TimeSpan> _sleepDurations = new []
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(8)
        };

        private readonly IObjectContainer _objectContainer;
        private readonly IObjectCreator _objectCreator;
        private readonly IActivitiesClient _activitiesClient;
        private readonly Context _context;

        public ActivitiesSteps(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
            _objectCreator = _objectContainer.Resolve<IObjectCreator>();
            _activitiesClient = _objectContainer.Resolve<IActivitiesClient>();
            _context = _objectContainer.Resolve<Context>();
        }

        private static Func<Type, string> CreatorName = (x) => {return x.GetType().ToString(); };

        private static Func<Type, string> CreatorUserRef = (x) => { return x.GetType().Assembly.GetName().Name; };

        [Given(@"a ([^ ]* message) for Account ([^ ]*) is published")]
        public void GivenAMessageIsPublished(Type messageType, string accountName)
        {
            var account = _context.GetAccount(accountName);
            var message = _objectCreator.Create(messageType, new { AccountId = account.Id, CreatorName = CreatorName(messageType), CreatorUserRef = CreatorUserRef(messageType) });
            var bus = _objectContainer.Resolve<IAzureTopicMessageBus>();

            bus.PublishAsync(message);
            account.IncrementMessageCount();
        }

        [Given(@"a ([^ ]* message) for Account ([^ ]*) and CreatedAt (\d+ days ago) is published")]
        public void GivenAMessageIsPublished(Type messageType, string accountName, DateTime createdAt)
        {
            var account = _context.GetAccount(accountName);
            var message = _objectCreator.Create(messageType, new { AccountId = account.Id, CreatedAt = createdAt, CreatorName = CreatorName(messageType), CreatorUserRef = CreatorUserRef(messageType) });
            var bus = _objectContainer.Resolve<IAzureTopicMessageBus>();

            bus.PublishAsync(message);
            account.IncrementMessageCount();
        }

        [Given(@"a ([^ ]* message) for Account ([^ ]*) and PayeScheme ([^ ]*) is published")]
        public void GivenAMessageIsPublished(Type messageType, string accountName, string payeScheme)
        {
            var account = _context.GetAccount(accountName);
            var message = _objectCreator.Create(messageType, new { AccountId = account.Id, PayeScheme = payeScheme, CreatorName = CreatorName(messageType), CreatorUserRef = CreatorUserRef(messageType) });
            var bus = _objectContainer.Resolve<IAzureTopicMessageBus>();

            bus.PublishAsync(message);
            account.IncrementMessageCount();
        }

        [When(@"I get latest activities for Account ([^ ]*)")]
        public async Task WhenIGetLatestActivities(string accountName)
        {
            var account = _context.GetAccount(accountName);

            _context.Set(account);

            await Task.WhenAll(_context.GetAccounts().Select(async a =>
            {
                var result = await Policy
                    .HandleResult<AggregatedActivitiesResult>(r => r.Total != a.MessageCount)
                    .WaitAndRetryAsync(_sleepDurations)
                    .ExecuteAsync(async () => await _activitiesClient.GetLatestActivities(a.Id));

                a.SetResult(result);
            }));
        }

        [When(@"I get activities for Account ([^ ]*)")]
        public async Task WhenIGetActivities(string accountName)
        {
            var account = _context.GetAccount(accountName);

            _context.Set(account);

            await Task.WhenAll(_context.GetAccounts().Select(async a =>
            {
                var result = await Policy
                    .HandleResult<ActivitiesResult>(r => r.Total != a.MessageCount)
                    .WaitAndRetryAsync(_sleepDurations)
                    .ExecuteAsync(async () => await _activitiesClient.GetActivities(new ActivitiesQuery
                    {
                        AccountId = a.Id
                    }));

                a.SetResult(result);
            }));
        }

        [When(@"I get activities for Account ([^ ]*) and Take (\d+)")]
        public async Task WhenIGetActivities(string accountName, int take)
        {
            var account = _context.GetAccount(accountName);

            _context.Set(account);

            await Task.WhenAll(_context.GetAccounts().Select(async a =>
            {
                var result = await Policy
                    .HandleResult<ActivitiesResult>(r => r.Total != a.MessageCount)
                    .WaitAndRetryAsync(_sleepDurations)
                    .ExecuteAsync(async () => await _activitiesClient.GetActivities(new ActivitiesQuery
                    {
                        AccountId = a.Id,
                        Take = take
                    }));

                a.SetResult(result);
            }));
        }

        [When(@"I get activities for Account ([^ ]*) and From (\d+ days ago) and To (\d+ days ago)")]
        public async Task WhenIGetActivities(string accountName, DateTime from, DateTime to)
        {
            var account = _context.GetAccount(accountName);

            _context.Set(account);
            
            await Task.WhenAll(_context.GetAccounts().Select(async a =>
            {
                var result = await Policy
                    .HandleResult<ActivitiesResult>(r => r.Total != a.MessageCount)
                    .WaitAndRetryAsync(_sleepDurations)
                    .ExecuteAsync(async () => await _activitiesClient.GetActivities(new ActivitiesQuery
                    {
                        AccountId = a.Id,
                        From = from,
                        To = to
                    }));

                a.SetResult(result);
            }));
        }

        [When(@"I get activities for Account ([^ ]*) and PayeScheme ([^ ]*)")]
        public async Task WhenIGetActivities(string accountName, string payeScheme)
        {
            var account = _context.GetAccount(accountName);

            _context.Set(account);
            
            await Task.WhenAll(_context.GetAccounts().Select(async a =>
            {
                var result = await Policy
                    .HandleResult<ActivitiesResult>(r => r.Total != a.MessageCount)
                    .WaitAndRetryAsync(_sleepDurations)
                    .ExecuteAsync(async () => await _activitiesClient.GetActivities(new ActivitiesQuery
                    {
                        AccountId = a.Id,
                        Data = new Dictionary<string, string>
                        {
                            ["PayeScheme"] = payeScheme
                        }
                    }));

                a.SetResult(result);
            }));
        }

        [Then(@"I should have (\d+) ([^ ]*) activities for (\d+) aggregations")]
        public void ThenIShouldHaveAnActivityAggregation(int count, ActivityType activityType, int aggregationCount)
        {
            var account = _context.Get<Account>();
            var result = account.GetResult<AggregatedActivitiesResult>();
            
            Check.That(result.Aggregates.Count(a => a.TopHit.Type == activityType && a.Count == count)).IsEqualTo(aggregationCount);
        }

        [Then(@"I should have (\d+) ([^ ]*) activities")]
        public void ThenIShouldHaveAnActivity(int count, ActivityType activityType)
        {
            var account = _context.Get<Account>();
            var result = account.GetResult<ActivitiesResult>();

            Check.That(result.Activities.Count(a => a.Type == activityType)).IsEqualTo(count);
        }
    }
}

