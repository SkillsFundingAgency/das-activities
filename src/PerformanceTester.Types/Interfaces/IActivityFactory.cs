using System;
using System.Collections.Generic;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Parameters;

namespace PerformanceTester.Types
{
    /// <summary>
    ///     A service that creates in memory activity objects. It does not store these anywhere.
    /// </summary>
    public interface IActivityFactory
    {
        Activity CreateRandomActivity(long accountId, DateTime at);
        IEnumerable<AccountActivities> CreateActivitiesByAccount(PopulateActivitiesParameters populateActivitiesParameters);
        IEnumerable<Activity> CreateActivities(PopulateActivitiesParameters populateActivitiesParameters);
    }
}