using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Activities.Application.Queries.GetActivities;
using SFA.DAS.Activities.Application.Validation;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityRequestValidator
    {
        public ValidationResult Validate(SaveActivityCommand item)
        {
            var validationResults = new ValidationResult();

            if (string.IsNullOrEmpty(item.Activity.OwnerId))
            {
                validationResults.AddError(nameof(item.Activity.OwnerId), "Cannot get Activity when owner ID is not given.");
            }

            return validationResults;
        }

        public Task<ValidationResult> ValidateAsync(GetActivitiesByOwnerIdRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}
