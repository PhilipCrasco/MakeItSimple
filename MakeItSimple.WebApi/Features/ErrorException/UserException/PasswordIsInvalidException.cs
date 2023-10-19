namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class PasswordIsInvalidException : Exception
    {
        public PasswordIsInvalidException() : base("Password is invalid!") { }
    }
}
