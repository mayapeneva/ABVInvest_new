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
            rssAddresses = [Constants.CapitalRSSCompanies, Constants.CapitalRSSMarkets, Constants.CapitalRSSFinance, Constants.InvestorRSSCompanies, Constants.InvestorRSSMarkets, Constants.InvestorRSSFinance, Constants.X3NewsRSS];
        }

        public IEnumerable<RSSFeedViewModel> LoadNews()
        {
            var rssFeedModels = new List<RSSFeedViewModel>();

            foreach (var rss in rssAddresses)
            {
                try
                {
                    //WebClient webClient = new WebClient();
                    //webClient.Headers.Add("user-agent", "MyApplication/v1.0 (http://abvinvest.eu)");
                    //webClient.Headers.Add("referer", "http://abvinvest.eu");

                    //using XmlReader reader = XmlReader.Create(webClient.OpenRead("https://www.capital.bg/rss/"));

                    using var reader = XmlReader.Create(rss);
                    var feed = SyndicationFeed.Load(reader);
                    reader.Close();

                    foreach (SyndicationItem item in feed.Items)
                    {
                        var publishDate = item.PublishDate.UtcDateTime; // DateTime.Parse(item.PublishDate.UtcDateTime);
                        if (publishDate > this.twoWeeksBackDate)
                        {
                            rssFeedModels.Add(new RSSFeedViewModel
                            {
                                Title = item.Title?.Text,
                                Uri = item.Links[0]?.Uri.ToString(),
                                PublishedDate = publishDate,
                                Summary = item.Summary?.ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return rssFeedModels;
        }
    }
}
