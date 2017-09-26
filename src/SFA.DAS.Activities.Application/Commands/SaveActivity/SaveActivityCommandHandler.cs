using SFA.DAS.Activities.Application.Exceptions;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Application.Validation;
using SFA.DAS.Activities.Domain.Models;
using SFA.DAS.Activities.Domain.Repositories;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityCommandHandler : IAsyncRequestHandler<SaveActivityCommand, SaveActivityCommandResponse>
    {
        private readonly IActivitiesRepository _repository;
        private readonly IValidator<SaveActivityCommand> _validator;

        public SaveActivityCommandHandler(IActivitiesRepository repository, IValidator<SaveActivityCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<SaveActivityCommandResponse> Handle(SaveActivityCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var activity = await _repository.GetActivity(message.OwnerId, message.Type) ?? new Activity
            {
                OwnerId = message.OwnerId,
                Type = message.Type,
                Description = message.Description
            };

            await _repository.SaveActivity(activity);

            return new SaveActivityCommandResponse();
        }
    }
}
