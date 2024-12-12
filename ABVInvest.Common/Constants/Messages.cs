namespace ABVInvest.Common.Constants
{
    public static class Messages
    {
        public static class Common
        {
            public const string CouldNotUploadInformation = "Файлът, който качихте е празен или информацията в него е с неправилен формат.";
            public const string DateError = "Моля, въведете дата с правилен формат.";
            public const string FileError = "Моля, изберете файл с правилен XML формат.";
            public const string NoBalance = "Няма налична информация за баланса Ви към тази дата. Моля, изберете дата след 01-01-2016г. и не по-късна от {0}г.";
            public const string RequiredField = "Полето е задължително";
        }

        public static class Data
        {
            public const string BfbCodeError = "БФБ кодът трябва да е дълъг 3 или 4 символа и да съдържа цифри и/или главни латински букви.";
            public const string CurrencyAdded = "Валута с код {0} е добавена.";
            public const string CurrencyCodeError = "Кодът на валутата трябва да е дълъг 3 символа и да съдържа само главни латински букви.";
            public const string CurrencyExists = "Валута с този код вече съществува.";
            public const string CurrencyDataIsWrong = "Въведените данни относно валутата не са правилни.";
            public const string IsinError = "ISIN кодът трябва да е дълъг 12 символа и да съдържа главни латински букви и цифри.";
            public const string IssuerDataIsWrong = "Въведените данни относно емитента не са правилни.";
            public const string MarketAdded = "Пазар с име {0} е добавен.";
            public const string MarketExists = "Пазар с това име или код вече съществува.";
            public const string MarketDataIsWrong = "Въведените данни относно пазара не са правилни.";
            public const string MarketMICError = "Идентификационният код на пазара трябва да е дълъг 4 символа и да съдържа само главни латински букви.";
            public const string SecurityAdded = "Ценни книжа с ISIN код {0} са добавен.";
            public const string SecurityExists = "Ценни книжа с този ISIN код вече съществуват.";
            public const string SecurityDataIsWrong = "Въведените данни относно ценните книжа не са правилни.";
            public const string WrongCurrencyData = "Валутният код се състои от 3 главни латински букви.";
            public const string WrongMarketData = "Името на пазара е неправилно.";
            public const string WrongSecurityData = "ISIN кодът се състои от 12 цифри или главни латински букви. БФБ кодът се състои от 3 или 4 цифри или главни латински букви.";
        }

        public static class DealsAndPortfolios
        {
            public const string CouldNotUploadInformation = "Файлът, който качихте е празен или информацията в него е с неправилен формат.";
            public const string CurrencyCannotBeCreated = "- Валута с код {0} за клиент с потребителско име {1} не може да бъдат регистрирана поради неправилен код. Валутата е за ценни книжа с Емитент: {2}, ISIN: {3}, БФБ код {4}.";
            public const string DailyDealsAlredyExist = "- Сделките за клиент с потребителско име {0} за дата {1} вече са регистрирани.";
            public const string DailyDealsCannotBeCreated = "- Сделките за клиент с потребителско име {0} не могат да бъдат регистрирани за дата {1} поради неправилни данни.";
            public const string DealCannotBeRegistered = "Сделка {0} с ценни книжа с ISIN: {1} за клиент с потребителско име {2} не може да бъде регистрирана поради неправилни данни: {3}: {4}.";
            public const string DealRowCannotBeCreated = "- Сделка {0} с ценни книжа с ISIN: {1} за клиент с потребителско име {2} не може да бъде регистрирана поради неправилни данни: Количество: {3}, Единична цена: {4}.";
            public const string MarketDoesNotExist = "- Сделка {0} с ценни книжа с ISIN: {1} за клиент с потребителско име {2} не може да бъде регистрирана поради неправилни данни: несъществуващ пазар с MIC код: {3}.";
            public const string NoPortfolio = "Няма налична информация за портфолиото Ви към тази дата.";
            public const string PortfolioCannotBeCreated = "- Портфолио за клиент с потребителско име {0} не може да бъдат създадено за дата {1} поради неправилни данни.";
            public const string SecurityCannotBeCreated = "- Ценни книжа за клиент с потребителско име {0} не могат да бъдат регистрирани поради неправилни данни: Емитент: {1}, ISIN: {2}, БФБ код {3}, Валута: {4}.";
            public const string SecurityCannotBeRegistered = "- Ценни книжа за клиент с потребителско име {0} не могат да бъдат регистрирани поради неправилни данни: {1}: {2}.";
            public const string SecurityExistsInThisPortfolio = "- Ценни книжа за клиент с потребителско име {0} не могат да бъдат регистрирани повторно за дата {1}: Емитент: {2}, ISIN: {3}, БФБ код {4}, Валута: {5}.";
            public const string UploadingSuccessfull = "Качването на информация приключи успешно.";
            public const string UploadingSumarry = "Резюме: Качването на информация приключи. Бяха изпълнени {0} промени.";
        }

        public static class User
        {
            public const string EmailError = "Моля въведете валиден имейл адрес.";
            public const string InvalidLogInAttempt = "Неуспешен опит за влизане в сайта.";
            public const string PINError = "ПИН кодът трябва да е дълъг 5 символа и да съдържа само цифри.";
            public const string UserDoesNotExist = "- Клиент с потребителско име {0} не е регистриран и всички записи за този клиент са пропуснати.";
            public const string UsernameError = "Потребителското име трябва да е дълго 5 или 10 символа и да съдържа цифри и/или главни латински букви.";
        }
    }
}
