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
    public class TagAndUnTagUserRole : ControllerBase
    {
        private readonly IMediator _mediator;
        public TagAndUnTagUserRole(IMediator mediator)
        {
            _mediator = mediator;
        
        }

        public class TagandUntagUserRoleCommand : IRequest<Unit>
        {
            public int user_role_id { get; set; }
            public ICollection<string> permission { get; set; }
            public string modified_by { get; set; }


        }

        public class Handler : IRequestHandler<TagandUntagUserRoleCommand, Unit>
        {

            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(TagandUntagUserRoleCommand command, CancellationToken cancellationToken)
            {
                var userRoles = await _context.UserRole.FirstOrDefaultAsync( x => x.Id == command.user_role_id , cancellationToken );
                
                if (userRoles == null)
                {
                    throw new UserRoleIdNotFoundException();
                }

                userRoles.Permissions = command.permission;
                userRoles.ModifiedBy = command.modified_by;
                userRoles.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
                 
            }

        }

        [HttpPut("TagAndUnTagUserRole/{id:int}")]
        public async Task<IActionResult> TagAndUnTagUserRoles([FromRoute] int id , [FromBody] TagandUntagUserRoleCommand command)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                command.user_role_id = id;
                command.modified_by = User.Identity?.Name;
                await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Messages.Add("Successfully update");
                return Ok(response);
            }
            catch (Exception e)
            {

                response.Success = false;
                response.Status = StatusCodes.Status409Conflict;
                response.Messages.Add(e.Message);
                return Conflict(response);

            }
        }

    }
}
