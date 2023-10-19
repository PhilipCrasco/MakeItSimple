using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Domain.Setup;
using MakeItSimple.WebApi.Features.ErrorException.SetupException.DepartmentException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace MakeItSimple.WebApi.Features.Setup.Departments
{
    [Route("api/Department")]
    [ApiController]
    public class AddDepartmentAsync : ControllerBase
    {

        private readonly IMediator _mediator;

        public AddDepartmentAsync(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class AddDepartmentCommand : IRequest<Unit>
        {
            public string department_name { get; set; }
            public int added_by { get; set; }

        }

        public class Handler : IRequestHandler<AddDepartmentCommand, Unit>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AddDepartmentCommand command, CancellationToken cancellationToken)
            {

                var DepartmentAlreadyExist = await _context.Departments
                    .FirstOrDefaultAsync(x => x.DepartmentName == command.department_name, cancellationToken);

                if (DepartmentAlreadyExist != null)
                {
                    throw new DepartmentAlreadyExistException();
                }

                var department = new Department
                {
                    DepartmentName = command.department_name,
                    AddedBy = command.added_by,
                    IsActive = true
                };

                await _context.AddAsync(department, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }


        }


        [HttpPost("AddNewDepartment")]
        public async Task<IActionResult> AddDepartment(AddDepartmentAsync.AddDepartmentCommand command)
        {

            var response = new CommandOrQueryResult<object>();

            try
            {
                if (User.Identity is ClaimsIdentity identity
                   && int.TryParse(identity.FindFirst("id")?.Value, out int userId))
                {
                    command.added_by = userId;
                }

                var result = await _mediator.Send(command);
                response.Status = StatusCodes.Status200OK;
                response.Success= true;
                response.Data = result;
                response.Messages.Add("Department successfully added");
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
