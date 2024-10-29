using ABVInvest.Common.ViewModels;

namespace ABVInvest.Common.Parsers
{
    public interface IRSSFeedService
    {
        Task<IEnumerable<RSSFeedViewModel>> LoadNews();
    }
}
