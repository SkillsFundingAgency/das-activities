using MediatR;
using SFA.DAS.Activities.Domain.Models;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityCommand : IAsyncRequest<SaveActivityCommandResponse>
    {
        public Activity ActivityPayload { get; set; }
        //public ActivityType Type { get; set; }
        //public string AccountId { get; set; }
        //public string Description { get; set; }
        //public string Url { get; set; }
    }
}
