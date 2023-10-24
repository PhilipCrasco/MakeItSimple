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
    public class ChangePasswordUser : ControllerBase
    {

        private readonly IMediator _mediator;

        public ChangePasswordUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class ChangePasswordUserCommand : IRequest<Unit>
        {
            public int user_id { get; set; }
            public string old_password { get; set; }
            public string new_password { get; set;}
            public string confirm_password { get; set; }
            public string modified_by { get; set; }

        }

        public class Handler : IRequestHandler<ChangePasswordUserCommand, Unit>
        {

            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(ChangePasswordUserCommand command, CancellationToken cancellationToken)
            {

                var users = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.user_id ,cancellationToken);

                if (users == null)
                {
                    throw new UserIdNotFoundException();
                }

                if(!BCrypt.Net.BCrypt.Verify(command.old_password , users.Password))
                {
                    throw new OldPasswordIncorrectException();
                }
                
                if(command.new_password != command.confirm_password )
                {
                    throw new NewPasswordNotEqualToConPasswordException();
                }

                if(command.new_password == string.Empty || command.confirm_password == string.Empty)
                {
                    throw new RequiredFieldMustBeFillException();
                }

                users.Password = BCrypt.Net.BCrypt.HashPassword(command.new_password);

                
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }


        [HttpPut("ChangePassword/{id:int}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id , ChangePasswordUserCommand command)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                command.user_id = id;
                command.modified_by = User.Identity.Name;
                await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Messages.Add("Successfully change password");
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
