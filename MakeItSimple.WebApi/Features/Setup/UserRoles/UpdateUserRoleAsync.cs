using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Features.ErrorException.SetupException.UserRoleException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Features.Setup.UserRoles
{
    [Route("api/UserRole")]
    [ApiController]
    public class UpdateUserRoleAsync : ControllerBase
    {
        private readonly IMediator _mediator;
        public UpdateUserRoleAsync(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class UpdateUserRoleCommand : IRequest<Unit>
        {
            public int user_role_id { get; set; }
            public string role_name { get; set; }
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

                var userRoleNotExist = await _context.UserRole.FirstOrDefaultAsync(x => x.UserRoleName == command.role_name, cancellationToken);

                if (userRoles == null)
                {
                    throw new UserRoleIdNotFoundException();
                }

                if (userRoles.UserRoleName == command.role_name)
                {
                    throw new NoChangesException();
                }


                if (userRoleNotExist != null)
                {
                    throw new UserRoleAlreadyExistException();
                }

                userRoles.UserRoleName = command.role_name;
                userRoles.ModifiedBy = command.modified_by;
                userRoles.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync(cancellationToken);

               
                return Unit.Value;
            }
        }


        [HttpPut("UpdateUserRole/{id:int}")]
        public async Task<IActionResult> UpdateUserRole([FromRoute] int id , [FromBody] UpdateUserRoleCommand command)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                command.user_role_id = id;
                command.modified_by = User.Identity?.Name;
                await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Messages.Add("Successfully update user role");
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
