using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Features.ErrorException.UserException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Claims;
using ZstdSharp.Unsafe;

namespace MakeItSimple.WebApi.Features.Users
{
    [Route("api/User")]
    [ApiController]
    public class AddUserAsync : ControllerBase
    {

        private readonly IMediator _mediator;       


        public AddUserAsync(IMediator mediator)
        {
            _mediator = mediator;        
        }

        public class AddNewUserCommand : IRequest<Unit>
        {

            public string firstname { get; set; }

            public string lastname { get; set; }

            public string username { get; set; }

            public string password { get; set; }

            public string confirm_password { get; set; }

            public int ? added_by { get; set; }

            public int? user_role_id { get; set; }

            public int? department_id { get; set; }


        }

        public class Handler : IRequestHandler<AddNewUserCommand, Unit>
        {

                private readonly DataContext _context;

                public Handler(DataContext context)
                {
                    _context = context;
                }


                public async Task<Unit> Handle(AddNewUserCommand command, CancellationToken cancellationToken)
                {
                    var UsernameAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Username == command.username, cancellationToken);

                    var DepartmentNotFound = await _context.Departments.AnyAsync(x => x.Id == command.department_id, cancellationToken);

                    var UserRoleNotFound = await _context.UserRole.AnyAsync(x => x.Id == command.user_role_id, cancellationToken);

                if (UsernameAlreadyExist != null)
                {
                    throw new UserAlreadyExistException(command.username);
                }

                if (!DepartmentNotFound && command.department_id.HasValue)
                    {
                        throw new DepartmentNotFoundException();
                    }

                    if(!UserRoleNotFound && command.user_role_id.HasValue)
                    {
                        throw new UserRoleNotFoundException();
                    }

                    if(command.password != command.confirm_password || command.password == null || command.confirm_password == null)
                    {
                        throw new PasswordIsInvalidException();
                    }


                    var user = new User
                    {

                        Firstname = command.firstname,
                        Lastname = command.lastname,
                        Username = command.username,
                        Password = BCrypt.Net.BCrypt.HashPassword(command.password),
                        UserRoleId = command.user_role_id,
                        AddedBy  = command.added_by,
                        DepartmentId = command.department_id,
                        
                    };


                    await _context.Users.AddAsync(user , cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);


                    return Unit.Value;
                }
        }


 


        [HttpPost("AddNewUser")]
        public async Task<IActionResult> AddUser([FromBody] AddNewUserCommand command)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                if (User.Identity is ClaimsIdentity identity
                && int.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.added_by = userId;
                }
                var result = await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Data = result;
                response.Messages.Add("User added Successfully");
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
