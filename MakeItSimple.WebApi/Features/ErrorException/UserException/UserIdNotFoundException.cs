namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class UserIdNotFoundException : Exception
    {

        public UserIdNotFoundException() : base("User id doesn't exist!") { }
    }
}
