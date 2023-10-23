namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class NewPasswordNotEqualToConPasswordException : Exception
    {
        public NewPasswordNotEqualToConPasswordException() :base ("New password are not equal to confirm password!"){ }
    }
}
