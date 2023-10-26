using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Features.ErrorException.SetupException.DepartmentException;
using MakeItSimple.WebApi.Features.ErrorException.SetupException.UserRoleException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Features.Setup.Departments
{
    [Route("api/Department")]
    [ApiController]
    public class UpdateDepartmentAsync : ControllerBase
    {
        private readonly IMediator _mediator;

        public UpdateDepartmentAsync(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class UpdateDepartmentAsyncCommand : IRequest<Unit>
        {
            public int department_id { get; set; }
            public string department_name { get; set; }
            public string modified_by { get; set; }

        }

        public class Handler : IRequestHandler<UpdateDepartmentAsyncCommand, Unit>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateDepartmentAsyncCommand command, CancellationToken cancellationToken)
            {

                var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.department_id, cancellationToken);
                
                var departmentAlreadyExist = await _context.Departments.FirstOrDefaultAsync(x => x.DepartmentName == command.department_name, cancellationToken);

                if(department == null)
                {
                    throw new DepartmentIdNotFoundException();
                }

                if(departmentAlreadyExist.DepartmentName == command.department_name)
                {
                    throw new NoChangesException();
                }

                if (departmentAlreadyExist != null)
                {
                    throw new DepartmentAlreadyExistException();
                }
                

                department.DepartmentName = command.department_name;
                department.ModifiedBy = command.modified_by;
                department.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

        [HttpPut("UpdateDepartment/{id:int}")]
        public async Task<IActionResult> UpdateDepartments([FromRoute] int id , [FromBody] UpdateDepartmentAsyncCommand command)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                command.department_id = id;
                command.modified_by = User.Identity.Name;
                await _mediator.Send(command);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Messages.Add("Successfully update department");
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
