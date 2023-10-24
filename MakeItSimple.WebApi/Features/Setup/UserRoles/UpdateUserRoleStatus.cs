using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Features.ErrorException.SetupException.UserRoleException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Features.Setup.UserRoles
{
    [Route("api/UserRole")]
    [ApiController]
    public class UpdateUserRoleStatus : ControllerBase
    {

        private readonly IMediator _mediator;

        public UpdateUserRoleStatus(IMediator mediator)
        {
            _mediator = mediator;   
        }

        public class UpdateUserRoleCommand : IRequest<Unit>
        {
            public int user_role_id { get; set; }

            public string modified_by { get; set; }

        }

        public class Handler : IRequestHandler<UpdateUserRoleCommand, Unit>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateUserRoleCommand command, CancellationToken cancellationToken)
            {

                var userRoles = await _context.UserRole.FirstOrDefaultAsync(x => x.Id == command.user_role_id, cancellationToken);

                if(userRoles == null)
                {
                    throw new UserRoleIdNotFoundException();

                }

                if(userRoles.IsActive == true && userRoles.Permissions.Count <= 0)
                {
                    throw new UserRoleDeactivationException();
                }

                userRoles.IsActive = !userRoles.IsActive;
                userRoles.ModifiedBy = command.modified_by;
                userRoles.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }

        [HttpPut("UpdateUserRoleStatus/{id:int}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                var command = new UpdateUserRoleCommand
                {
                    user_role_id = id,
                    modified_by = User.Identity?.Name,
                };

                await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Messages.Add("Successfully update status");
                return Ok(response);
            }
            catch(Exception e)
            {
                response.Success = false;
                response.Status = StatusCodes.Status409Conflict;
                response.Messages.Add(e.Message);
                return Conflict(response);
            }
        }


    }
}
