using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Application.Exceptions;
using SFA.DAS.Activities.Application.Validation;
using SFA.DAS.Activities.Domain.Repositories;

namespace SFA.DAS.Activities.Application.Commands
{
    public class GenericCommandHandler<TCommand, TResponse> : IAsyncRequestHandler<TCommand, TResponse> where TCommand : Command<TResponse> where TResponse : new()
    {
        private readonly IActivitiesRepository _repository;
        private readonly IValidator<TCommand> _validator;

        public GenericCommandHandler(IActivitiesRepository repository, IValidator<TCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async  Task<TResponse> Handle(TCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _repository.SaveActivity(command.PayLoad);

            return new TResponse();
        }
    }
}
