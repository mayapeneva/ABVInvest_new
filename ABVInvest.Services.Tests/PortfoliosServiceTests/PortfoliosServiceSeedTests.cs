using ABVInvest.Common.Constants;
using ABVInvest.Data;
using ABVInvest.Services.Portfolios;
using System.Security.Claims;
using Xunit;

namespace ABVInvest.Services.Tests.PortfoliosServiceTests
{
    public class PortfoliosServiceSeedTests : IDisposable
    {
        private readonly ApplicationDbContext Db;
        private readonly IPortfoliosService PortfoliosService;

        public PortfoliosServiceSeedTests()
        {
            var moqUser = TestHelper.UserSetupForPortfoliosTests();
            var principal = new ClaimsPrincipal();
            (PortfoliosService, Db) = TestHelper.PortfoliosServiceSetup(principal, moqUser);

            if (!Db.ApplicationUsers.Any(u => u.UserName == Constants.UserNameOne)) Db.SeedUser(Constants.UserNameOne);
            if (!Db.ApplicationUsers.Any(u => u.UserName == Constants.UserNameTwo)) Db.SeedUser(Constants.UserNameTwo);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldCreatePortfolioForThisDateForEachUserInFile()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 01));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, string.Empty));
            var expectedBooleanResult = true;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var user1HasPortfolioForTheDate = Db.ApplicationUsers
                .SingleOrDefault(u => u.UserName == Constants.UserNameOne)?
                .Portfolio.Any(p => p.Date == date);
            var user2HasPortfolioForTheDate = Db.ApplicationUsers
                .SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?
                .Portfolio.Any(p => p.Date == date);

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            Assert.Equal(expectedBooleanResult, user1HasPortfolioForTheDate);
            Assert.Equal(expectedBooleanResult, user2HasPortfolioForTheDate);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreatePortfolioForNonExistingUser()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 02));
            var userName = "0000000002";
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "2"));

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var usersPortfolio = Db.DailySecuritiesPerClient.SingleOrDefault(ds => ds.ApplicationUser.UserName == userName);

            // Assert
            Assert.False(actualResult.IsSuccessful());
            Assert.Equal([string.Format(Messages.User.UserDoesNotExist, userName)], actualResult.Errors);
            Assert.Null(usersPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateSecondProtfolioForUserForTheSameDate()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 03));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, string.Empty));

            var firstResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            Assert.True(firstResult.IsSuccessful());

            var expectedPortfoliosCount = 1;
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var actualPortfoliosCount = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameOne)?.Portfolio.Count(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Equal(expectedPortfoliosCount, actualPortfoliosCount);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldAddUsersFullNameIfThereIsNone()
        {
            // Arange
            var userName = "0000000008";
            Db.SeedUser(userName);
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 04));
            var expectedUserFullName = "ИНДЪСТРИ ДИВЕЛЪПМЪНТ ХОЛДИНГ АД";
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "3"));

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var user = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == userName);

            // Assert
            Assert.True(actualResult?.IsSuccessful());
            Assert.Equal(expectedUserFullName, user?.FullName);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldCreateSecurityIfItDoesNotExist()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 05));
            var securityISIN = "BG1100019980";
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "4"));

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var securities = Db.Securities;

            // Assert
            Assert.True(actualResult?.IsSuccessful());
            Assert.Contains(securities, s => s.ISIN == securityISIN);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfSecurityHasWrongISIN()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 06));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "5"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotMakePortfolioRowEntryIfSecurityAlreadyEnteredInThisDailyPortfolio()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 07));
            var securityISIN = "BG1100026985";
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, string.Empty));
            var expectedSecurityEntryCount = 1;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var actualSecurityEntryCount = (Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameOne)?.Portfolio.SingleOrDefault(p => p.Date == date)?.SecuritiesPerIssuerCollection.Select(s => s.Security.ISIN))?.Count(n => n == securityISIN);

            // Assert
            Assert.True(actualResult?.IsSuccessful());
            Assert.Equal(expectedSecurityEntryCount, actualSecurityEntryCount);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldCreateCurrencyIfItDoesNotExist()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 08));
            var currencyCode = "EUR";
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "6"));

            var initialCurrencies = Db.Currencies;
            Assert.DoesNotContain(initialCurrencies, c => c.Code == currencyCode);

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var currencies = Db.Currencies;

            // Assert
            Assert.True(actualResult?.IsSuccessful());
            Assert.Contains(currencies, c => c.Code == currencyCode);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfCurrencyHasWrongCode()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 09));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "7"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameOne)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfQuantityCoultNotBeParsed()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 10));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "8"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfOpenPriceCoultNotBeParsed()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 11));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "9"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfMarketPriceCoultNotBeParsed()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 12));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "10"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfMarketValueCoultNotBeParsed()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 13));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "11"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfResultCoultNotBeParsed()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 14));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "12"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfResultBGNCoultNotBeParsed()
        {
            // Arange
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "13"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, TestHelper.PortfoliosDate);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == TestHelper.PortfoliosDate);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfYieldPercentCoultNotBeParsed()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 16));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "14"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task SeedPortfoliosAsync_ShouldNotCreateEntryIfRelativePartCoultNotBeParsed()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 17));
            var deserialisedPortfolios = TestHelper.DeserialisePortfolios(string.Format(Constants.FileNamePortfolios, "15"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await PortfoliosService.SeedPortfoliosAsync(deserialisedPortfolios, date);
            var dailyPortfolio = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyPortfolio);
        }

        public void Dispose() => Db?.Dispose();
    }
}
