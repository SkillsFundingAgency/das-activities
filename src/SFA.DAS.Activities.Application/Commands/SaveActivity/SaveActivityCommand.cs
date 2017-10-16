using System;
using MediatR;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityCommand : IAsyncRequest<SaveActivityCommandResponse>
    {
        public SaveActivityCommand(Activity activity)
        {
            Activity = activity;
        }

        public Activity Activity { get; private set; }
    }
}
