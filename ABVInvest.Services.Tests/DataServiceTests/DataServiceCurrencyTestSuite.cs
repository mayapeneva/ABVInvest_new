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
        private ApplicationDbContext Db;
        private IDataService DataService;

        public DataServiceCurrencyTestSuite()
        {
            (DataService, Db) = TestHelper.DataServiceSetup();
        }

        [Fact]
        public async Task CreateCurrencyAsync_ShouldCreateCurrency()
        {
            // Arrange            
            var expectedCurrenciesCount = 1;

            // Act
            var actualResult = await DataService.CreateCurrencyAsync(Constants.CurrencyCode);
            var actualCurrenciesCount = Db.Currencies.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(Constants.CurrencyCode, data.Code);
            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
            Assert.Contains(Db.Currencies, c => c.Code == Constants.CurrencyCode);
        }

        [Fact]
        public async Task CreateCurrencyAsync_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            await DataService.CreateCurrencyAsync(Constants.CurrencyCode);
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyExists);

            // Act
            var actualResult = await DataService.CreateCurrencyAsync(Constants.CurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task CreateCurrencyAsync_ShouldNotCreateCurrencyIfCodeNotCorrect()
        {
            // Arrange
            var wrongCurrencyCode = Constants.Test;
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyDataIsWrong);

            // Act
            var actualResult = await DataService.CreateCurrencyAsync(wrongCurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task GetOrCreateCurrencyAsync_ShouldGetCurrencyIfExists()
        {
            // Arrange
            await DataService.CreateCurrencyAsync(Constants.CurrencyCode);

            // Act
            var actualResult = await DataService.GetOrCreateCurrencyAsync(Constants.CurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(Constants.CurrencyCode, actualResult.Code);
        }

        [Fact]
        public async Task GetOrCreateCurrencyAsync_ShouldCreateCurrencyIfSuchDoesNotExist()
        {
            // Arrange
            var expectedCurrenciesCount = 1;

            // Act
            var actualResult = await DataService.GetOrCreateCurrencyAsync(Constants.CurrencyCode);
            var actualCurrenciesCount = Db.Currencies.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(Constants.CurrencyCode, actualResult.Code);

            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
            Assert.Contains(Db.Currencies, c => c.Code == Constants.CurrencyCode);
        }

        public void Dispose() => Db?.Dispose();
    }
}
