namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    /// <summary>
    ///     Represents a container for any data that is required by the activity repositories to perform paging.
    /// </summary>
    /// <remarks>
    ///     Cosmos uses continuation tokens.
    ///     Elastic uses skip / take.
    /// </remarks>
    public interface IPagingData
    {
        bool MoreDataAvailable { get; }
        int RequiredPageSize { get; set; }
        int CurrentPageSize { get; set; }
        int Inspections { get; set; }
        IActivityDocumentRepository Repository { get; }
        int? MaximumInspections { get; }
    }
}