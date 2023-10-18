using Google.Protobuf.WellKnownTypes;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Features.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserAsync : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetUserAsync(IMediator mediator)
        {
            _mediator = mediator;
        }


        public class GetUserAsyncQuery : UserParams, IRequest<PagedList<GetUserAsyncQueryResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }

        }


        public class GetUserAsyncQueryResult
        {
            public int id { get; set; }

            public string firstname { get; set; }

            public string lastname { get; set; }

            public string username { get; set; }

            public string password { get; set; }

            public string addedby { get; set; }

            public DateTime created_at { get; set; }

            public bool isactive { get; set; }

            public string department_name { get; set; }

            public string role_name { get; set; }

            public ICollection<string> permission { get; set; }

        }


        public class Handler : IRequestHandler<GetUserAsyncQuery, PagedList<GetUserAsyncQueryResult>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetUserAsyncQueryResult>> Handle(GetUserAsyncQuery request, CancellationToken cancellationToken)
            {
                IQueryable<User> users = _context.Users
                    .Include(x => x.AddedByUser)
                    .Include(x => x.UserRole)
                    .Include(x => x.Department);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    users = users.Where(x => x.Firstname.Contains(request.Search) ||
                                        x.Lastname.Contains(request.Search)
                                      ||x.Username.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    users = users.Where(x => x.IsActive ==  request.Status);
                }

                var defaultPermissions = new List<string>();

                var result = users.Select(x => new GetUserAsyncQueryResult
                {
                    id = x.Id,
                    firstname = x.Firstname,
                    lastname = x.Lastname,
                    username = x.Username,
                    password = x.Password,
                    addedby = x.AddedByUser.Username != null ? x.AddedByUser.Username : "",
                    created_at = x.CreatedAt,
                    isactive = x.IsActive,
                    department_name = x.Department.DepartmentName != null ? x.Department.DepartmentName : "",
                    role_name = x.UserRole.UserRoleName != null ? x.UserRole.UserRoleName : "",
                    permission = x.UserRole.Permissions != null ? x.UserRole.Permissions : defaultPermissions

                });

                return await PagedList<GetUserAsyncQueryResult>.CreateAsync(result, request.PageNumber, request.PageSize);

            }
        }


        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] GetUserAsync.GetUserAsyncQuery query)
        {

            var response = new CommandOrQueryResult<object>();

            try
            {
                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(
                      users.CurrentPage,
                      users.PageSize,
                      users.TotalCount,
                      users.TotalPages,
                      users.HasPreviousPage,
                      users.HasNextPage
                    );

                var result = new CommandOrQueryResult<object>()
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                    Data = new
                    {
                        users,
                        users.CurrentPage,
                        users.PageSize,
                        users.TotalCount,
                        users.TotalPages,
                        users.HasPreviousPage,
                        users.HasNextPage
                    },

              
            };

                result.Messages = new List<string>(response.Messages) { "successfully fetch data" };

                return Ok(result);

            }
            catch (Exception e)
            {
                response.Status = StatusCodes.Status409Conflict;
                response.Success = true;
                response.Messages.Add(e.Message);
                return Conflict(response);
            }

        }
        


    }


}
