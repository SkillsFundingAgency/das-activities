
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Application.Exceptions;
using SFA.DAS.Activities.Application.Validation;
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

        public async Task<SaveActivityCommandResponse> Handle(SaveActivityCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _repository.SaveActivity(command.Payload);

            return new SaveActivityCommandResponse();
        }
    }
}
