﻿using System.Threading.Tasks;
using SFA.DAS.Activities.Application.Validation;

namespace SFA.DAS.Activities.Application.Queries.GetActivities
{
    public class GetActivitiesByOwnerIdRequestValidator
    {
        public ValidationResult Validate(GetActivitiesByOwnerIdRequest item)
        {
            var validationResults = new ValidationResult();

            if (item.AccountId<=0)
            {
                validationResults.AddError(nameof(item.AccountId), "Cannot get Activity when owner ID is not given.");
            }

            return validationResults;
        }

        public Task<ValidationResult> ValidateAsync(GetActivitiesByOwnerIdRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}
