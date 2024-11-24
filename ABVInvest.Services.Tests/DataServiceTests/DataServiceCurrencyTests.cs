using ABVInvest.Common;
using ABVInvest.Common.Constants;
using ABVInvest.Data.Models;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceCurrencyTests : BaseDataServiceTests
    {
        private const string CurrencyCode = "USD";

        [Fact]
        public async Task CreateCurrency_ShouldCreateCurrency()
        {
            // Arrange
            var expectedCurrenciesCount = 1;

            // Act
            var actualResult = await DataService.CreateCurrency(CurrencyCode);
            var actualCurrenciesCount = Db.Currencies.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(CurrencyCode, data.Code);
            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            await DataService.CreateCurrency(CurrencyCode);
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyExists);

            // Act
            var actualResult = await DataService.CreateCurrency(CurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfCodeNotCorrect()
        {
            // Arrange
            var wrongCurrencyCode = "US1";
            var expectedResult = new ApplicationResult<Currency>();
            expectedResult.Errors.Add(Messages.Data.CurrencyDataIsWrong);

            // Act
            var actualResult = await DataService.CreateCurrency(wrongCurrencyCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }
    }
}
