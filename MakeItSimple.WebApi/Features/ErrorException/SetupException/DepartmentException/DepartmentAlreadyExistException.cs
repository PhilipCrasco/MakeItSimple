namespace MakeItSimple.WebApi.Features.ErrorException.SetupException.DepartmentException
{
    public class DepartmentAlreadyExistException : Exception
    {
        public DepartmentAlreadyExistException() : base("Department name already exist!") { }
    }
}
