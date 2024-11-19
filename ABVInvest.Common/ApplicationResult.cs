namespace ABVInvest.Common
{
    public class ApplicationResult<T>
    {
        public T? Data { get; set; }

        public ICollection<string> Errors { get; set; } = [];

        public bool IsSuccessful() => this.Errors.Count == 0;
    }
}
