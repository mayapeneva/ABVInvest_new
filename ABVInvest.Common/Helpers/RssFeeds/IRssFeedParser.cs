using ABVInvest.Common.ViewModels;

namespace ABVInvest.Common.Helpers.RssFeeds
{
    public interface IRssFeedParser
    {
        Task<IEnumerable<RssFeedViewModel>> LoadNewsAsync();
    }
}
