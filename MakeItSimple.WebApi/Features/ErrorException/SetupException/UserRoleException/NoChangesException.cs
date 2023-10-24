namespace MakeItSimple.WebApi.Features.ErrorException.SetupException.UserRoleException
{
    public class NoChangesException : Exception
    {
        public NoChangesException() : base("No changes has been commit!") { }
    }
}
