namespace ABVInvest.Common
{
    public static class Constants
    {
        public static class RSSFeed
        {
            public const string CapitalRSSCompanies = "https://www.capital.bg/rss/?rubrid=2268/";
            public const string CapitalRSSMarkets = "https://www.capital.bg/rss/?rubrid=3060/";
            public const string CapitalRSSFinance = "https://www.capital.bg/rss/?rubrid=2272/";

            public const string InvestorRSSCompanies = "https://www.investor.bg/rss/c/528-kompanii";
            public const string InvestorRSSMarkets = "https://www.investor.bg/rss/c/10-borsa";
            public const string InvestorRSSFinance = "https://www.investor.bg/rss/c/536-finansi";

            public const string X3NewsRSS = "https://www.x3news.com/?page=rssfeed&language=bg";
        }
        
        public static class User
        {
            public const string PINRegex = "^\\d{5}$";
            public const string UserNameRegex = "^[A-Z0-9]{5}$|^[A-Z0-9]{10}$";
        }

        public static class Role
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
    }
}
