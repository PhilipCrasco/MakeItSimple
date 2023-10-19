using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Features.ErrorException.SetupException.UserRoleException;
using MakeItSimple.WebApi.Features.ErrorException.UserException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MakeItSimple.WebApi.Features.Setup.UserRoles
{
    [Route("api/UserRole")]
    [ApiController]
    public class AddUserRoleAsync : ControllerBase
    {
        private readonly IMediator _mediator;
        public AddUserRoleAsync(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class AddUserRoleCommand : IRequest<Unit>
        {
            public string role_name { get; set; }

            public List<string> permissions { get; set; }

            public int added_by { get; set; }


        }

        public class Handler : IRequestHandler<AddUserRoleCommand, Unit>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AddUserRoleCommand command, CancellationToken cancellationToken)
            {
                var UserRoleAlreadyExist = await _context.UserRole
                    .FirstOrDefaultAsync(x => x.UserRoleName == command.role_name ,cancellationToken);

                if(UserRoleAlreadyExist != null )
                {
                    throw new UserRoleAlreadyExistException();
                }

                var userRole = new UserRole
                {
                    UserRoleName = command.role_name,
                    Permissions = command.permissions,
                    AddedBy = command.added_by,
                    IsActive = true
                };

                await _context.AddAsync(userRole , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;

            }
        }

        [HttpPost("AddNewUserRole")]
        public async Task<IActionResult> AddUserRole([FromBody] AddUserRoleCommand command)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                if(User.Identity is ClaimsIdentity identity
                    && int.TryParse(identity.FindFirst("id")?.Value , out int userId))
                {
                    command.added_by = userId;
                }

                var result = await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Data = result;
                response.Messages.Add("UserRole added Successfully");
                return Ok(response);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Messages.Add(e.Message);
                return Conflict(response);
            }
        }



    }
}
