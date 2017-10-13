using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Application.Exceptions;
using SFA.DAS.Activities.Application.Validation;
using SFA.DAS.Activities.Domain.Repositories;

namespace SFA.DAS.Activities.Application.Commands.CommitmentHasBeenApproved
{
    public class CommitmentHasBeenApprovedCommandHandler : IAsyncRequestHandler<CommitmentHasBeenApprovedCommand, CommitmentHasBeenApprovedCommandResponse>
    {
        private readonly IActivitiesRepository _repository;
        private readonly IValidator<CommitmentHasBeenApprovedCommand> _validator;

        public CommitmentHasBeenApprovedCommandHandler(IActivitiesRepository repository, IValidator<CommitmentHasBeenApprovedCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<CommitmentHasBeenApprovedCommandResponse> Handle(CommitmentHasBeenApprovedCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _repository.SaveActivity(command.PayLoad);

            return new CommitmentHasBeenApprovedCommandResponse();
        }
    }
}
