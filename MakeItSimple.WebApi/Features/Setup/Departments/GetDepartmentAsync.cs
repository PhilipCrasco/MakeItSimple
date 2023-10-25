using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.Domain.Setup;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;

namespace MakeItSimple.WebApi.Features.Setup.Departments
{
    [Route("api/Department")]
    [ApiController]
    public class GetDepartmentAsync : ControllerBase
    {

        private readonly IMediator _mediator;

        public GetDepartmentAsync(IMediator mediator)
        {
             _mediator = mediator;
        }


        public class GetDepartmentResult
        {
            public int id { get; set; }
            public string department_name { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public bool isactive { get; set; }
            public int? added_by { get; set; }
            public string modified_by { get; set; }

            public List<Users> user { get; set; }

            public class Users
            {
                public int user_id { get; set; }
                public string username { get; set; }

            }
            

        }

        public class GetDepartmentQuery : UserParams, IRequest<PagedList<GetDepartmentResult>>
        {
            public bool? status { get; set; }
            public string search { get; set; }
        }

        public class Handler : IRequestHandler<GetDepartmentQuery, PagedList<GetDepartmentResult>>
        {

            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public Task<PagedList<GetDepartmentResult>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Department> departments = _context.Departments.Include(x => x.AddedByUser);

                if (!string.IsNullOrEmpty(request.search))
                {
                    departments = departments.Where(d => d.DepartmentName.Contains(request.search));
                }

                if (request.status != null)
                {
                    departments = departments.Where(d => d.IsActive == request.status);
                }

                var result = departments.Select(x => new GetDepartmentAsync.GetDepartmentResult
                {

                    id = x.Id,
                    department_name = x.DepartmentName,
                    added_by = x.AddedBy,
                    created_at = x.CreatedAt,
                    updated_at = x.UpdatedAt,
                    isactive = x.IsActive,
                    modified_by = x.ModifiedBy != null ? x.ModifiedBy : "",
                    user = x.Users.Select(x => new GetDepartmentResult.Users
                    {
                        user_id = x.Id,
                        username = x.Username,

                    }).ToList(),

                });


                return PagedList<GetDepartmentResult>.CreateAsync(result, request.PageNumber , request.PageSize);

            }
        }

        [HttpGet("GetDepartment")]
        public async Task<IActionResult> GetDepartments([FromQuery] GetDepartmentQuery command)
        {
            var response = new CommandOrQueryResult<object>();

            try
            {
                var department = await _mediator.Send(command);

                Response.AddPaginationHeader(
                    department.CurrentPage,
                    department.PageSize,
                    department.TotalCount,
                    department.TotalPages,
                    department.HasPreviousPage,
                    department.HasNextPage
                );

                var results = new CommandOrQueryResult<object>
                {
                    Success = true,
                    Data = new
                    {
                        department,
                        department.CurrentPage,
                        department.PageSize,
                        department.TotalCount,
                        department.TotalPages,
                        department.HasPreviousPage,
                        department.HasNextPage
                    }
                };

                results.Messages = new List<string>(response.Messages) { "successfully fetch data" };
                return Ok(results);
            }
            catch (System.Exception e)
            {
                response.Success = false;
                response.Status = StatusCodes.Status409Conflict;
                response.Messages.Add(e.Message);
                return Conflict(e.Message);
            }
        }

    }
}
