namespace SFA.DAS.Activities.IntegrityChecker
{
    public class ActivityScanParams
    {
        public ActivityScanParams()
        {
            ScanBatchSize = 500;
        }

        public int ScanBatchSize { get; set; }
        public int? MaxInspections { get; set; }
        public int? MaxDiscrepancies { get; set; }
    }
}