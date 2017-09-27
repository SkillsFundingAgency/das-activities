using MediatR;
using NuGetProject.Enums;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityCommand : IAsyncRequest<SaveActivityCommandResponse>
    { 
        public ActivityType Type { get; set; }
        public string AccountId { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
