using System.Collections.Generic;
using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public class ActivityDiscrepancyFinderParameters
    {
        public int BatchSize { get; set; }
        public int? MaxInspections { get; set; }
        public IFixActionReaderLogger ReaderLogger { get; set; }
    }

    /// <summary>
    ///     Service that can find activity discrepancies. Discrepancies are returned with a <see cref="ActivityDiscrepancyType"/>
    ///     that indicates the issues that afflict the activity.
    /// </summary>
    public interface IActivityDiscrepancyFinder
    {
        IEnumerable<ActivityDiscrepancy> Scan(ActivityDiscrepancyFinderParameters parameters);
    }
}