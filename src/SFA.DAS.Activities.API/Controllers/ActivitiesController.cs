using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.Activities.API.Attributes;
using SFA.DAS.Activities.Application.Queries;
using SFA.DAS.Activities.Domain.Models;

namespace SFA.DAS.Activities.API.Controllers
{
    [RoutePrefix("api/activities/{ownerId}")]
    public class ActivitiesController : ApiController
    {
        private readonly IMediator _mediator;

        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        [ApiAuthorize(Roles = "ReadOwnerTasks")]
        public async Task<IHttpActionResult> GetTasks(string ownerId)
        {
            var result = await _mediator.SendAsync(new GetActivitiesByOwnerIdRequest(ownerId));

            if (result?.Activities == null) return Ok(Enumerable.Empty<Activity>());

            return Ok(result);
        }
    }
}
