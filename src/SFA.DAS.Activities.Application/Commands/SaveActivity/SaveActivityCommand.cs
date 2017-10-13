using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NuGet;

namespace SFA.DAS.Activities.Application.Commands.SaveActivity
{
    public class SaveActivityCommand : IAsyncRequest<SaveActivityCommandResponse>
    {
        public Activity Payload { get; set; }
    }
}
