using ABVInvest.Common.Models;

namespace ABVInvest.Common.Parsers
{
    public interface IRSSFeedService
    {
        Task<IEnumerable<RSSFeedViewModel>> LoadNews();
    }
}
