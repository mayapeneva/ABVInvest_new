using ABVInvest.Common.Models;

namespace ABVInvest.Common.Parsers
{
    public interface IRSSFeedParser
    {
        IEnumerable<RSSFeedViewModel> LoadNews();
    }
}
