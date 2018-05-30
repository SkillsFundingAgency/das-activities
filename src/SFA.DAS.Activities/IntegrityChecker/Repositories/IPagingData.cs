using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    /// <summary>
    ///     Represents a container for any data that is required by the container to perform paging.
    /// </summary>
    /// <remarks>
    ///     Cosmos uses continuation tokens.
    ///     Elastic uses skip / take.
    /// </remarks>
    public interface IPagingData
    {
        bool MoreDataAvailable { get; }
        int RequiredPageSize { get; set; }
        int CurrentPage { get; set; }
        int Inspections { get; set; }
        IActivityDocumentRepository Repository { get; }
        int? MaximumInspections { get; }
    }
}