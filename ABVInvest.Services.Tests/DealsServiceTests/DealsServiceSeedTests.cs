using ABVInvest.Common.Constants;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Deals;
using System.Security.Claims;
using Xunit;

namespace ABVInvest.Services.Tests.DealsServiceTests
{
    public class DealsServiceSeedTests : IDisposable
    {
        private readonly ApplicationDbContext Db;
        private readonly IDealsService DealsService;

        public DealsServiceSeedTests()
        {
            var moqUser = TestHelper.UserSetupForDealsTests();
            var principal = new ClaimsPrincipal();
            (DealsService, Db) = TestHelper.DealsServiceSetup(principal, moqUser);

            if (!Db.ApplicationUsers.Any(u => u.UserName == Constants.UserNameOne)) Db.SeedUser(Constants.UserNameOne);
            if (!Db.ApplicationUsers.Any(u => u.UserName == Constants.UserNameTwo)) Db.SeedUser(Constants.UserNameTwo);

            if (!Db.Markets.Any(m => m.MIC == "XBUL"))
            {
                Db.Markets.Add(new Market { Name = "БФБ", MIC = "XBUL" });
                Db.SaveChanges();
            }
        }

        [Fact]
        public async Task SeedDeals_ShouldCreateDealCollectionForThisDateForEachUserInFile()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 11, 01));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, string.Empty));
            var expectedBooleanResult = true;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var user1HasDealsForTheDate = Db.ApplicationUsers
                .SingleOrDefault(u => u.UserName == Constants.UserNameOne)?
                .Deals.Any(p => p.Date == date);
            var user2HasDealsForTheDate = Db.ApplicationUsers
                .SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?
                .Deals.Any(p => p.Date == date);

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            Assert.Equal(expectedBooleanResult, user1HasDealsForTheDate);
            Assert.Equal(expectedBooleanResult, user2HasDealsForTheDate);
        }

        [Fact]
        public async Task SeedDeals__ShouldNotCreateDealCollectionForNonExistingUser()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 02));
            var userName = "0000000002";
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "2"));

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var usersDeals = Db.DailyDeals.SingleOrDefault(dd => dd.ApplicationUser.UserName == userName);

            // Assert
            Assert.False(actualResult.IsSuccessful());
            Assert.Equal([string.Format(Messages.User.UserDoesNotExist, userName)], actualResult.Errors);
            Assert.Null(usersDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateSecondDealsCollectionForUserForTheSameDate()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 03));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, string.Empty));

            var firstResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            Assert.True(firstResult.IsSuccessful());

            var expectedDealsCount = 1;
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var actualDealsCount = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameOne)?.Deals.Count(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Equal(expectedDealsCount, actualDealsCount);
        }

        [Fact]
        public async Task SeedDeals_ShouldCreateSecurityIfItDoesNotExist()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 04));
            var securityISIN = "BG1100041067";
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "3"));

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var securities = Db.Securities;

            // Assert
            Assert.True(actualResult?.IsSuccessful());
            Assert.Contains(securities, s => s.ISIN == securityISIN);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfSecurityHasWrongISIN()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 05));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "4"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldCreateCurrencyIfItDoesNotExist()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 06));
            var currencyCode = "EUR";
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "5"));

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var currencies = Db.Currencies;

            // Assert
            Assert.True(actualResult?.IsSuccessful());
            Assert.Contains(currencies, c => c.Code == currencyCode);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfCurrencyHasWrongCode()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 07));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "6"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameOne)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfMarketDoesNotExist()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 08));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "7"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameOne)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfDealTypeNotValid()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 09));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "8"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfSettlementDateNotValid()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 10));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "9"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfSinglePriceCoultNotBeParsed()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 11));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "10"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfShareCountCoultNotBeParsed()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 12));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "11"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfCouponCoultNotBeParsed()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 13));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "12"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfAmountInShareCurrencyCoultNotBeParsed()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 14));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "13"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfAmountInPaymentCurrencyCoultNotBeParsed()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 15));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "14"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task SeedDeals_ShouldNotCreateEntryIfCommissionCoultNotBeParsed()
        {
            // Arrange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 16));
            var deserializedDeals = await TestHelper.DeserialiseDeals(string.Format(Constants.FileNameDeals, "15"));
            var expectedErrorsCount = 2;

            // Act
            var actualResult = await DealsService.SeedDealsAsync(deserializedDeals, date);
            var dailyDeals = Db.ApplicationUsers.SingleOrDefault(u => u.UserName == Constants.UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.False(actualResult?.IsSuccessful());
            Assert.Equal(expectedErrorsCount, actualResult?.Errors.Count);
            Assert.Null(dailyDeals);
        }

        public void Dispose() => Db?.Dispose();
    }
}
