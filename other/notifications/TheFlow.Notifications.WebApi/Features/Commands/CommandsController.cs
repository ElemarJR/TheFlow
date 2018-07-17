using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;
using TheFlow.Notifications.Application.CommandSide.MarkAsProcessed;
using TheFlow.Notifications.Application.CommandSide.NewMessage;
using TheFlow.Notifications.Application.Infrastructure.Mediatr.Commands;
using TheFlow.Notifications.WebApi.Swagger.Commands;

namespace TheFlow.Notifications.WebApi.Features.Commands
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class CommandsController : Controller
    {
        private readonly IMediator _mediator;

        public CommandsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("NewMessage")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Guid), "New messaging process instance Id.")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerRequestExample(typeof(NewMessageCommand), typeof(NewMessageCommandExample))]
        public async Task<IActionResult> NewMessage(
            [FromBody] NewMessageCommand command,
            [FromHeader(Name = "x-requestid")] string requestId
        )
        {
            if (!Guid.TryParse(requestId, out var guid))
            {
                return BadRequest();
            }

            var identifiedCommand = new IdentifiedCommand<NewMessageCommand, string>(
                command,
                guid
            );

            var result = await _mediator.Send(identifiedCommand);

            return (result == null)
                ? (IActionResult) BadRequest()
                : Ok(result);
        }


        [HttpPost]
        [Route("MarkAsProcessed")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Guid), "Process instance Id.")]
        [SwaggerResponse((int) HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, null, "Specified process not found or not expecting this command.")]
        [SwaggerRequestExample(typeof(MarkAsProcessedCommand), typeof(MarkAsProcessedCommandExample))]
        public async Task<IActionResult> MarkAsProcessed(
            [FromBody] MarkAsProcessedCommand command,
            [FromHeader(Name = "x-requestid")] string requestId
        )
        {
            if (!Guid.TryParse(requestId, out var guid))
            {
                return BadRequest();
            }

            var identifiedCommand = new IdentifiedCommand<MarkAsProcessedCommand, string>(
                command,
                guid
            );

            var result = await _mediator.Send(identifiedCommand);

            return (result == null)
                ? (IActionResult)NotFound()
                : Ok(result);
        }

    }
}
