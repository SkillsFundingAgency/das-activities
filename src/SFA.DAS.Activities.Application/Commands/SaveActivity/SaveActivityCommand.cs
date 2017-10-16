using System;
using MediatR;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityCommand : IAsyncRequest<SaveActivityCommandResponse>
    {
        public string OwnerId { get; set; }

        public string ActivityType { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public DateTime PostedDateTime { get; set; }
    }
}
