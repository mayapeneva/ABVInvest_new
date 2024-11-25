using ABVInvest.Common;
using ABVInvest.Common.Constants;
using ABVInvest.Data.Models;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceCurrencyTests
    {
        [Fact]
        public async Task CreateCurrency_ShouldCreateCurrency()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var expectedCurrenciesCount = 1;

            // Act
            var actualResult = await dataService.CreateCurrency(Constants.CurrencyCode);
            var actualCurrenciesCount = db.Currencies.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(Constants.CurrencyCode, data.Code);
            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);

            db.Dispose();
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            await dataService.CreateCurrency(Constants.CurrencyCode);
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyExists);

            // Act
            var actualResult = await dataService.CreateCurrency(Constants.CurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            db.Dispose();
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfCodeNotCorrect()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var wrongCurrencyCode = Constants.Test;
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyDataIsWrong);

            // Act
            var actualResult = await dataService.CreateCurrency(wrongCurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            db.Dispose();
        }
    }
}
