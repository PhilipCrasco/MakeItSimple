namespace MakeItSimple.WebApi.Features.ErrorException.SetupException.UserRoleException
{
    public class UserRoleIdNotFoundException : Exception
    {
        public UserRoleIdNotFoundException() :base("UserRole not found!"){ }
    }
}
