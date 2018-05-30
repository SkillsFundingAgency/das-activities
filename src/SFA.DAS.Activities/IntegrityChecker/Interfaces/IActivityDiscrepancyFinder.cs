using System.Collections.Generic;
using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    /// <summary>
    ///     Service that can find activity discrepancies. Discrepancies are returned with a <see cref="ActivityDiscrepancyType"/>
    ///     that indicates the issues that afflict the activity.
    /// </summary>
    public interface IActivityDiscrepancyFinder
    {
        IEnumerable<ActivityDiscrepancy> Scan(int batchSize);
        IEnumerable<ActivityDiscrepancy> Scan(int batchSize, int? maxInspections);
    }
}