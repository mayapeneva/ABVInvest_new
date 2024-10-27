using ABVInvest.Common.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ABVInvest.Common.Parsers
{
    public class RSSFeedService : IRSSFeedService
    {
        private readonly DateTime twoWeeksBackDate;
        private readonly ICollection<string> rssAddresses;
        private readonly HttpClient httpClient;

        public RSSFeedService(IHttpClientFactory clientFactory)
        {
            this.httpClient = clientFactory.CreateClient();

            twoWeeksBackDate = DateTime.UtcNow.Subtract(new TimeSpan(14, 0, 0, 0));
            rssAddresses = [Constants.RSSFeed.InvestorRSSCompanies, Constants.RSSFeed.InvestorRSSMarkets, Constants.RSSFeed.InvestorRSSFinance, Constants.RSSFeed.X3NewsRSS];
        }

        public async Task<IEnumerable<RSSFeedViewModel>> LoadNews()
        {
            var rssFeedModels = new List<RSSFeedViewModel>();

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
                        if (publishDate > this.twoWeeksBackDate)
                        {
                            rssFeedModels.Add(new RSSFeedViewModel
                            {
                                Title = item.Title?.Text,
                                Uri = item.Links[0]?.Uri?.ToString(),
                                PublishedDate = publishDate,
                                Summary = item.Summary?.Text
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: log the exception
                }
            }

            return rssFeedModels.OrderByDescending(m => m.PublishedDate);
        }
    }
}
