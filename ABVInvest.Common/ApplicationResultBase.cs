namespace ABVInvest.Common
{
    public class ApplicationResultBase
    {
        public ICollection<string> Errors { get; set; } = [];

        public bool IsSuccessful() => this.Errors.Count == 0;
    }
}
