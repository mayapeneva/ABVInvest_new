using ABVInvest.Common.ViewModels;

namespace ABVInvest.Services.News
{
    public interface IRSSFeedService
    {
        Task<IEnumerable<RSSFeedViewModel>> LoadNewsAsync();
    }
}
