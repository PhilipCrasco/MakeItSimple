using MakeItSimple.WebApi.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;

namespace MakeItSimple.WebApi.Features.Authentication;

    public class MyAuthenticationFailureHandler : JwtBearerEvents
    {
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var responseObj = new CommandOrQueryResult<object>
            {
                Status = StatusCodes.Status401Unauthorized,
                Success = false,
                Messages = new List<string> { "You are not authorized to access this resource." }
            };
            var responseText = JsonSerializer.Serialize(responseObj);
            return context.Response.WriteAsync(responseText);
        }

    }

