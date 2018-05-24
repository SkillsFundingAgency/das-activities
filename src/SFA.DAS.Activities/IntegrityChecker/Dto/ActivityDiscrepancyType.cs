using System;

namespace SFA.DAS.Activities.IntegrityChecker
{
    [Flags]
    public enum ActivityDiscrepancyType
    {
        None = 0,
        NotFoundInCosmos = 1,
        NotFoundInElastic = 2
    }
}