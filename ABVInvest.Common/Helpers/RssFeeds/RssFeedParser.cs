using ABVInvest.Common.Constants;
using ABVInvest.Common.ViewModels;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ABVInvest.Common.Helpers.RssFeeds
{
    public class RssFeedParser : IRssFeedParser
    {
        private readonly DateTime twoWeeksBackDate;
        private readonly ICollection<string> rssAddresses;
        private readonly HttpClient httpClient;

        public RssFeedParser(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient();

            twoWeeksBackDate = DateTime.UtcNow.Subtract(new TimeSpan(14, 0, 0, 0));
            rssAddresses = [ShortConstants.RssFeed.InvestorRssCompanies, ShortConstants.RssFeed.InvestorRssMarkets, ShortConstants.RssFeed.InvestorRssFinance, ShortConstants.RssFeed.X3NewsRss];
        }

        public async Task<IEnumerable<RssFeedViewModel>> LoadNewsAsync()
        {
            var rssFeedModels = new List<RssFeedViewModel>();

            foreach (var rss in rssAddresses)
            {
                try
                {
                    using var response = await httpClient.GetAsync(rss);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var xmlReader = XmlReader.Create(stream);
                    var feed = SyndicationFeed.Load(xmlReader);
                    xmlReader.Close();

                    foreach (SyndicationItem item in feed.Items)
                    {
                        var publishDate = item.PublishDate.UtcDateTime;
                        if (publishDate > twoWeeksBackDate)
                        {
                            rssFeedModels.Add(new RssFeedViewModel
                            {
                                Title = item.Title?.Text,
                                Uri = item.Links[0]?.Uri?.ToString()!,
                                PublishedDate = publishDate,
                                Summary = item.Summary?.Text
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    // TODO: log the exception
                }
            }

            return rssFeedModels.OrderByDescending(m => m.PublishedDate);
        }
    }
}
