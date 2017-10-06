using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using NuGetProject;
using SFA.DAS.Activities.Domain.Models;


namespace SFA.DAS.Activities.Worker.MessageProcessors
{
    //CreateActivityMessage to be added to shared code outside of the solution. Currently in nuget project
    public class CreateActivityMessageMessageProcessor : MessageProcessor<CreateActivityMessage>
    {
        private readonly IMediator _mediator;

        public CreateActivityMessageMessageProcessor(IPollingMessageReceiver pollingMessageReceiver, ILog logger, IMediator mediator) : base(pollingMessageReceiver, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(CreateActivityMessage message)
        {
            await _mediator.SendAsync(new SaveActivityCommand
                {
                    ActivityPayload = new Activity()
                        {
                            AccountId = message.AccountId,
                            Type = message.Type,
                            Description = message.Description,
                            Url = message.Url
                        }
                }
            );
        }
    }
}