using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Features.ErrorException.UserException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Features.Users
{
    [Route("api/User")]
    [ApiController]
    public class UpdateUserAsync : ControllerBase
    {
        private readonly IMediator _mediator;
        public UpdateUserAsync(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class UpdateUserCommand : IRequest<Unit>
        {
            public int user_id { get; set; }

            public string firstname { get; set; }

            public string lastname { get; set; }

            public string username { get; set; }

            public string modified_by { get; set; }

            public int user_role_id { get; set; } 

            public int department_id { get; set; }

        }

        public class Handler : IRequestHandler<UpdateUserCommand, Unit>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

             public  async Task<Unit> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
            {


                var users = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.user_id, cancellationToken);

                var userRoleNotFound = await _context.UserRole.FirstOrDefaultAsync(x => x.Id == command.user_role_id , cancellationToken);

                var departmentNotFound = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.department_id , cancellationToken);   

                
                if(users == null)
                {
                    throw new UserIdNotFoundException();
                }

                if(userRoleNotFound == null)
                {
                    throw new UserRoleNotFoundException();
                }

                if (departmentNotFound == null)
                {
                    throw new DepartmentNotFoundException();
                }

                var usernameAlreadyExist = await _context.Users.FirstOrDefaultAsync(x => x.Username == command.username, cancellationToken);

                if(usernameAlreadyExist != null && users.Username != command.username)
                {
                    throw new UserAlreadyExistException(command.username);
                }

                users.Firstname = command.firstname;
                users.Lastname = command.lastname;
                users.Username = command.username;
                users.DepartmentId = command.department_id;
                users.UserRoleId = command.user_role_id;
                users.UpdatedAt = DateTime.Now;


                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }


        }

        [HttpPut("UpdateUser/{id:int}")]
        public async Task<IActionResult> UpdateUser([FromRoute]int id, [FromBody] UpdateUserCommand command)
        {

            var response = new CommandOrQueryResult<object>();
            try
            {
                command.user_id = id;
                command.modified_by = User.Identity.Name;
                await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Messages.Add("User has been updated successfully");
                return Ok(response);
            }
            catch(Exception e)
            {
                response.Status = StatusCodes.Status409Conflict;
                response.Messages.Add(e.Message);
                return Conflict(response);

            }

            

        }




    }
}
