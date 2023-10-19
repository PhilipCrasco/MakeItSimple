namespace MakeItSimple.WebApi.Features.ErrorException.SetupException.UserRoleException
{
    public class UserRoleAlreadyExistException: Exception
    {
        public UserRoleAlreadyExistException() : base("UserRole already exist!"){ }
    }
}
