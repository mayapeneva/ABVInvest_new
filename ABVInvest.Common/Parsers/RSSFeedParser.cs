using ABVInvest.Common.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ABVInvest.Common.Parsers
{
    public class RSSFeedParser : IRSSFeedParser
    {
        private readonly DateTime twoWeeksBackDate;
        private readonly ICollection<string> rssAddresses;

        public RSSFeedParser()
        {
            twoWeeksBackDate = DateTime.UtcNow.Subtract(new TimeSpan(14, 0, 0, 0));
            rssAddresses = [Constants.RSSFeed.InvestorRSSCompanies, Constants.RSSFeed.InvestorRSSMarkets, Constants.RSSFeed.InvestorRSSFinance, Constants.RSSFeed.X3NewsRSS];
        }

        public IEnumerable<RSSFeedViewModel> LoadNews()
        {
            var rssFeedModels = new List<RSSFeedViewModel>();

            foreach (var rss in rssAddresses)
            {
                try
                {
                    using var reader = XmlReader.Create(rss);
                    var feed = SyndicationFeed.Load(reader);
                    reader.Close();

                    foreach (SyndicationItem item in feed.Items)
                    {
                        var publishDate = item.PublishDate.UtcDateTime;
                        if (publishDate > this.twoWeeksBackDate)
                        {
                            rssFeedModels.Add(new RSSFeedViewModel
                            {
                                Title = item.Title?.Text,
                                Uri = item.Links[0]?.Uri.ToString(),
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
