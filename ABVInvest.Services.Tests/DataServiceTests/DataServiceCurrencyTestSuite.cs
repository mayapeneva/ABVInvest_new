using ABVInvest.Common;
using ABVInvest.Common.Constants;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Data;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceCurrencyTestSuite : IDisposable
    {
        private IDataService DataService;
        private ApplicationDbContext Db;

        public DataServiceCurrencyTestSuite()
        {
            (DataService, Db) = TestExtensions.DataServiceSetup();
        }

        [Fact]
        public async Task CreateCurrency_ShouldCreateCurrency()
        {
            // Arrange            
            var expectedCurrenciesCount = 1;

            // Act
            var actualResult = await DataService.CreateCurrency(Constants.CurrencyCode);
            var actualCurrenciesCount = Db.Currencies.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(Constants.CurrencyCode, data.Code);
            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
            Assert.Contains(Db.Currencies, c => c.Code == Constants.CurrencyCode);

            Db.Dispose();
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            await DataService.CreateCurrency(Constants.CurrencyCode);
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyExists);

            // Act
            var actualResult = await DataService.CreateCurrency(Constants.CurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            Db.Dispose();
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfCodeNotCorrect()
        {
            // Arrange
            var wrongCurrencyCode = Constants.Test;
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyDataIsWrong);

            // Act
            var actualResult = await DataService.CreateCurrency(wrongCurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            Db.Dispose();
        }

        [Fact]
        public async Task GetOrCreateCurrency_ShouldGetCurrencyIfExists()
        {
            // Arrange
            await DataService.CreateCurrency(Constants.CurrencyCode);

            // Act
            var actualResult = await DataService.GetOrCreateCurrency(Constants.CurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(Constants.CurrencyCode, actualResult.Code);

            Db.Dispose();
        }

        [Fact]
        public async Task GetOrCreateCurrency_ShouldCreateCurrencyIfSuchDoesNotExist()
        {
            // Arrange
            var expectedCurrenciesCount = 1;

            // Act
            var actualResult = await DataService.GetOrCreateCurrency(Constants.CurrencyCode);
            var actualCurrenciesCount = Db.Currencies.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(Constants.CurrencyCode, actualResult.Code);

            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
            Assert.Contains(Db.Currencies, c => c.Code == Constants.CurrencyCode);

            Db.Dispose();
        }

        public void Dispose() => Db?.Dispose();
    }
}
