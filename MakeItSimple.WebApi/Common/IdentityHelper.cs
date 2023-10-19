using System.Security.Claims;

namespace MakeItSimple.WebApi.Common
{
    public class IdentityHelper
    {
        public static bool TryGetUserId(ClaimsIdentity identity, out int userId)
        {
            return int.TryParse(identity.FindFirst("id")?.Value, out userId);
        }
    }
}
