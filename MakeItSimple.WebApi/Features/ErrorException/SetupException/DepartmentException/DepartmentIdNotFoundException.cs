namespace MakeItSimple.WebApi.Features.ErrorException.SetupException.DepartmentException
{
    public class DepartmentIdNotFoundException : Exception
    {
        public DepartmentIdNotFoundException() : base("department id not found!") { }
    }
}
