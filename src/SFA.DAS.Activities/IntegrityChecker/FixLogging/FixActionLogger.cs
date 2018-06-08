using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.FixLogging
{
    public class FixActionLogger : IFixActionLogger, IFixActionReaderLogger
    {
        public FixActionLogger()
        {
            Start();    
        }

        private readonly ConcurrentBag<FixActionLoggerItem> _fixes = new ConcurrentBag<FixActionLoggerItem>();
        private int _cosmosActivitiesFound;
        private int _elasticActivitiesFound;
        public void Add(FixActionLoggerItem item)
        {
            _fixes.Add(item);
        }

        public int CosmosActivitiesFound => _cosmosActivitiesFound;
        public int ElasticActivitiesFound => _elasticActivitiesFound;

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimeSpan Elapsed => EndTime - StartTime;

        public int FixesApplied => _fixes.Count;
        public int FixFailures{ get; private set; }
        public int FixSuccesses { get; private set; }

        public string TerminalException { get; set; }

        public FixActionLoggerExceptionSummary[] ExceptionSummary { get; private set; }

        public IEnumerable<FixActionLoggerItem> Fixes => _fixes;

        public void Start()
        {
            StartTime = DateTime.UtcNow;
        }

        public void IncrementCosmosActivities(int newActivitiesFound)
        {
            Interlocked.Add(ref _cosmosActivitiesFound, newActivitiesFound);
        }

        public void IncrementElasticActivities(int newActivitiesFound)
        {
            Interlocked.Add(ref _elasticActivitiesFound, newActivitiesFound);
        }

        public void Finish()
        {
            EndTime = DateTime.UtcNow;

            FixFailures = Fixes.Count(i => !i.Success);
            FixSuccesses = _fixes.Count - FixFailures;
            ExceptionSummary = _fixes
                .Where(i => !i.Success)
                .SelectMany(i => i.HandledBy.Select(h => h.Error))
                .GroupBy(s => s)
                .Select(grp => new FixActionLoggerExceptionSummary {Exception = grp.Key, Occurrences = grp.Count()})
                .ToArray();
        }
    }
}