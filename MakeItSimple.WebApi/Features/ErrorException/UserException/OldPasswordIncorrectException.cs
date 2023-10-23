namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class OldPasswordIncorrectException: Exception
    {
        public OldPasswordIncorrectException() :base("Old password is incorrect!") { }
    }
}
