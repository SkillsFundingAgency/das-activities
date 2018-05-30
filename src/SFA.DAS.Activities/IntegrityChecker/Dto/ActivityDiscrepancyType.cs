using System;

namespace SFA.DAS.Activities.IntegrityChecker.Dto
{
    [Flags]
    public enum ActivityDiscrepancyType
    {
        None = 0,
        NotFoundInCosmos = 1,
        NotFoundInElastic = 2
    }
}