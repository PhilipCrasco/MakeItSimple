namespace MakeItSimple.WebApi.Features.ErrorException.AuthenticationException
{
    public class UsernamePasswordIncorrectException : Exception
    {
        public UsernamePasswordIncorrectException() : base("Username or Password is incorrect!") { }
    }
}
