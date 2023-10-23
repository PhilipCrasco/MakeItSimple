namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class RequiredFieldMustBeFillException : Exception
    {
        public RequiredFieldMustBeFillException() :base("Required field must be fill!") { }
    }
}
