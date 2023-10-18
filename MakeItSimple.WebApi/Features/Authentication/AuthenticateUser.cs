using AutoMapper;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Features.ErrorException.AuthenticationException;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Generators;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace MakeItSimple.WebApi.Features.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateUser : ControllerBase
    {

        private readonly IMediator _mediator;


        public AuthenticateUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class AuthenticateUserQuery : IRequest<AuthenticateUserResult>
        {
            public AuthenticateUserQuery(string username)
            {
                Username = username;
            }

            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }    

        }

        public class AuthenticateUserResult
        {

            public int id { get; set; }

            public string firstname { get; set; }

            public string lastname { get; set; }

            public string username { get; set; }

            public string role_name { get; set; }

            public ICollection<string> permission { get; set;}

            public string token { get; set; }


            public AuthenticateUserResult(User user , string Token)
            {
                id = user.Id;
                firstname = user.Firstname; 
                lastname = user.Lastname;
                username = user.Username;
                token = Token;
                role_name = user.UserRole?.UserRoleName;
                permission = user.UserRole?.Permissions;

            }

           public class Handler : IRequestHandler<AuthenticateUserQuery, AuthenticateUserResult>
            {
                private readonly DataContext _context;
                private readonly IConfiguration _configuration;
                private readonly IMapper _mapper;

                public Handler(DataContext context , IConfiguration configuration , IMapper mapper)
                {
                    
                    _context = context;
                    _configuration =  configuration;
                    _mapper = mapper;

                }

                public async Task<AuthenticateUserResult> Handle(AuthenticateUserQuery command, CancellationToken cancellationToken)
                {
                    var user = await _context.Users.Include(x => x.UserRole)
                        .SingleOrDefaultAsync(x => x.Username == command.Username && x.IsActive == true, cancellationToken);
                    if (user == null || !BCrypt.Net.BCrypt.Verify(command.Username, user.Password))
                    {
                        throw new UsernamePasswordIncorrectException();
                    }

                    var Token = GenerateJwtToken(user);

                    var result = new AuthenticateUserResult(user , Token);

                    var results = _mapper.Map<AuthenticateUserResult>(result);
                    return results;

                }


                private string GenerateJwtToken(User user)
                {
                    var key = _configuration.GetValue<string>("JwtConfig:Key");
                    var Keybytes = Encoding.ASCII.GetBytes(key);
                    var audience = _configuration.GetValue<string>("JwtConfig:Audience");
                    var issuer = _configuration.GetValue<string>("JwtConfig:Issuer");
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                             new Claim("id", user.Id.ToString()),
                             new Claim(ClaimTypes.Name , user.Firstname),
                              new Claim(ClaimTypes.Name , user.Lastname)
                        }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Keybytes),
                        SecurityAlgorithms.HmacSha256Signature)

                    };

                    var Token = tokenHandler.CreateToken(tokenDescriptor);
                    return tokenHandler.WriteToken(Token);
                }


            }

        }


        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<ActionResult<AuthenticateUser.AuthenticateUserResult>> Authenticate(AuthenticateUser.AuthenticateUserQuery request)
        {
            var response = new CommandOrQueryResult<AuthenticateUser.AuthenticateUserResult>();
            try
            {
                var result = await _mediator.Send(request);
                response.Status = StatusCodes.Status200OK;
                response.Data = result;
                response.Success = true;
                response.Messages.Add("LogIn successfully");
                return Ok(response);

            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Messages.Add(ex.Message);
                return Conflict(response);
            }
            

        }


    }


}
