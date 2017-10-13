using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Application.Exceptions;
using SFA.DAS.Activities.Application.Validation;
using SFA.DAS.Activities.Domain.Repositories;

namespace SFA.DAS.Activities.Application.Queries.GetActivities
{
    public class
        GetActivitiesByOwnerIdHandler : IAsyncRequestHandler<GetActivitiesByOwnerIdRequest,
            GetActivitiesByOwnerIdResponse>
    {
        private readonly IActivitiesRepository _repository;
        private readonly IValidator<GetActivitiesByOwnerIdRequest> _validator;

        public GetActivitiesByOwnerIdHandler(IActivitiesRepository repository,
            IValidator<GetActivitiesByOwnerIdRequest> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GetActivitiesByOwnerIdResponse> Handle(GetActivitiesByOwnerIdRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var result = await _repository.GetActivities(message.OwnerId);

            return new GetActivitiesByOwnerIdResponse {Activities = result};
        }
    }
}
