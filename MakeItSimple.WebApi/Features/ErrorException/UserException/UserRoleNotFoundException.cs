namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class UserRoleNotFoundException : Exception
    {
        public UserRoleNotFoundException() :base("No UserRole found!") { }
    }
}
