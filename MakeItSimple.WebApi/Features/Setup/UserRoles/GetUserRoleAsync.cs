using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace MakeItSimple.WebApi.Features.Setup.UserRoles
{
    [Route("api/UserRole")]
    [ApiController]
    public class GetUserRoleAsync : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetUserRoleAsync(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class GetUserRoleAsyncResult
        {
            public int id { get; set; }
            public string role_name { get; set; }

            public ICollection<string> permissions { get; set; }

            public string added_by { get; set; }

            public DateTime created_at { get; set; }    
            public DateTime updated_at { get; set;}

            public bool isactive { get; set; }

            //public bool is_tagged { get; set; } 

            //public string user { get; set; }

        }


        public class GetUserRoleAsyncQuery : UserParams, IRequest<PagedList<GetUserRoleAsyncResult>>
        {
            public string search { get; set; }
            public bool? status { get; set; }
            //public bool? is_tagged { get; set; }

        }


        public class Handler : IRequestHandler<GetUserRoleAsyncQuery, PagedList<GetUserRoleAsyncResult>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetUserRoleAsyncResult>> Handle(GetUserRoleAsyncQuery request, CancellationToken cancellationToken)
            {
                IQueryable<UserRole> userRoles = _context.UserRole
                                                         .Include(x => x.AddedByUser)
                                                         .Include(x => x.User);

                if (!string.IsNullOrEmpty(request.search))
                {
                    userRoles = userRoles.Where(x => x.UserRoleName.Contains(request.search));
                
                
                }

                //if(request.is_tagged != null)
                //{
                //    userRoles = request.is_tagged.Value ? userRoles.Where(x => x.User != null) : userRoles.Where(x => x.User == null);
                //}

                if(request.status != null)
                {
                    userRoles = userRoles.Where(x => x.IsActive == request.status);
                }

                var userPermissions = new List<string>();

                var result = userRoles.Select(x => new GetUserRoleAsyncResult
                {
                     id = x.Id,
                     role_name  = x.UserRoleName,
                     permissions = x.Permissions != null ? x.Permissions : userPermissions,
                     added_by = x.AddedByUser.Username,
                     updated_at = x.UpdatedAt,
                     created_at = x.CreatedAt,
                     isactive = x.IsActive,
                     //is_tagged = x.User != null,
                    //user = x.User.Username
                });

                return await PagedList<GetUserRoleAsyncResult>.CreateAsync(result, request.PageNumber, request.PageSize);

            }
        }

        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles([FromQuery]GetUserRoleAsync.GetUserRoleAsyncQuery query)
        {
            var response = new CommandOrQueryResult<object>();
            try
            {
                var userRoles = await _mediator.Send(query);

                Response.AddPaginationHeader(
                    userRoles.CurrentPage,
                    userRoles.PageSize,
                    userRoles.TotalCount,
                    userRoles.TotalPages,
                    userRoles.HasPreviousPage,
                    userRoles.HasNextPage
                );

                var result = new CommandOrQueryResult<object>
                {
                    Success = true,
                    Status = StatusCodes.Status200OK,
                    Data = new
                    {
                        userRoles,
                        userRoles.CurrentPage,
                        userRoles.PageSize,
                        userRoles.TotalCount,
                        userRoles.TotalPages,
                        userRoles.HasPreviousPage,
                        userRoles.HasNextPage
                    }
                };

                response.Messages.Add("Successfully fetch data");
                return Ok(result);

            }
            catch (Exception e)
            {
                response.Status = StatusCodes.Status409Conflict;
                response.Messages.Add(e.Message);
                return Conflict(response);
            }
        }


    }
}
