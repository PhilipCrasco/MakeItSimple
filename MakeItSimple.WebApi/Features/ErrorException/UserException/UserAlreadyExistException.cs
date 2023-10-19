namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class UserAlreadyExistException : Exception
    {

        public UserAlreadyExistException(string username) : base($"{username} is already exist!") { }
    }
}
