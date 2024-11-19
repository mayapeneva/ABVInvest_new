namespace ABVInvest.Common.Constants
{
    public static class ShortConstants
    {
        public static class Common
        {
            public const string Error = "Грешка";
            public const string SvSeCulture = "sv-SE";
            public const string DateTimeParseFormat = "dd/MM/yyyy";
        }

        public static class Portfolios
        {
            public const string AveragePrice = "Средна цена";
            public const string MarketPrice = "Пазарна цена";
            public const string MarketValue = "Пазарна стойност";
            public const string PortfolioShare = "Тегло в портфейла";
            public const string Profit = "Доходност";
            public const string ProfitInPersentage = "Доходност в %";
            public const string Quantity = "Наличност";
        }

        public static class Role
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

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
    }
}
