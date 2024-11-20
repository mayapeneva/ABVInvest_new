namespace ABVInvest.Common
{
    public class ApplicationResult<T> : ApplicationResultBase
    {
        public T? Data { get; set; }
    }
}
