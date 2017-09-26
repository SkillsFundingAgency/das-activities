using MediatR;
using SFA.DAS.Activities.API.Types.Enums;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityCommand : IAsyncRequest<SaveActivityCommandResponse>
    {
        public ActivityType Type { get; set; }
        public string OwnerId { get; set; }
        public string Description { get; set; }
    }
}
