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

            if (item.Activity.AccountId<=0)
            {
                validationResults.AddError(nameof(item.Activity.AccountId), "Cannot save Activity when hashed ID is not given.");
            }

            return validationResults;
        }

        public Task<ValidationResult> ValidateAsync(GetActivitiesByOwnerIdRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}
