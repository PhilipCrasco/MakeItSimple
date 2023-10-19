namespace MakeItSimple.WebApi.Features.ErrorException.UserException
{
    public class DepartmentNotFoundException : Exception
    {
        public DepartmentNotFoundException() : base("No Department found!") { }
    }
}
