using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Features.ErrorException.UserException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Features.Users
{
    [Route("api/User")]
    [ApiController]
    public class UpdateUserStatus : ControllerBase
    {

        private readonly IMediator _mediator;
        public UpdateUserStatus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class UpdateUserStatusCommand : IRequest<Unit>
        {
            public int user_id { get; set; }

            public string modified_by { get; set; }

        }

        public class Handler : IRequestHandler<UpdateUserStatusCommand, Unit>
        {

            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateUserStatusCommand command, CancellationToken cancellationToken)
            {

                var users = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.user_id, cancellationToken);

                if (users == null)
                {
                    throw new UserIdNotFoundException();
                }

                users.IsActive = !users.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;

            }
        }

        [HttpPut("UpdateUserStatus/{id:int}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id)
        {

            var response = new CommandOrQueryResult<object>();
            try
            {
                var command = new UpdateUserStatusCommand
                {
                    user_id = id,
                    modified_by = User.Identity.Name
                };
                await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Messages.Add("Succesfully update status");
                return Ok(response);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Status = StatusCodes.Status409Conflict;
                response.Messages.Add(e.Message);
                return Conflict(e);
            }
        }

    }
}
