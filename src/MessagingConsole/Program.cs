using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MessagingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IMediator _mediator=new Mediator();

            _mediator.SendAsync(new SaveActivityCommand(
                new FluentActivity()
                    .OwnerId(message.OwnerId)
                    .ActivityType(Activity.ActivityType.ApprenticeChangesApproved)
                    .DescriptionSingular($"cohort approved with {message.ProviderName}")
                    .DescriptionPlural("cohorts approved")
                    .PostedDateTime(message.PostedDatedTime)
                    .Url(message.Url)
                    .AddAssociatedThing(message.ProviderName)
                    .AddAssociatedThings(message.Apprentices)
                    .Object()
            ));
        }
    }
}
