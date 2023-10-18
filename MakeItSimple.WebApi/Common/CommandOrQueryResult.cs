namespace MakeItSimple.WebApi.Common
{
    public class CommandOrQueryResult<T>
    {

        public int Status { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<string> Messages { get; set; } = new();

    }
}
