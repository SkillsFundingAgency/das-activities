using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NuGet;
using SFA.DAS.Activities.Application.Commands.CommitmentHasBeenApproved;

namespace SFA.DAS.Activities.Application.Commands
{
    public class Command<TResponse> : IAsyncRequest<TResponse>
    {
        public Activity PayLoad { get; set; }
    }
}
