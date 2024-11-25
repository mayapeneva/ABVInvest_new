using ABVInvest.Common;
using ABVInvest.Common.Constants;
using ABVInvest.Data.Models;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceMarketTests
    {
        [Fact]
        public async Task CreateMarket_ShouldCreateMarket()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var expectedMarketsCount = 1;

            // Act
            var actualResult = await dataService.CreateMarket(Constants.MarketName, Constants.MarketCode);
            var actualMarketsCount = db.Markets.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(Constants.MarketName, data.Name);
            Assert.Equal(Constants.MarketCode, data.MIC);
            Assert.Equal(expectedMarketsCount, actualMarketsCount);

            db.Dispose();
        }

        [Theory]
        [InlineData(Constants.MarketName, Constants.Test)]
        [InlineData(Constants.Test, Constants.MarketCode)]
        public async Task CreateMarket_ShouldNotCreateMarketIfSuchAlreadyExists(string marketName, string marketCode)
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            await dataService.CreateMarket(Constants.MarketName, Constants.MarketCode);
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketExists);

            // Act
            var actualResult = await dataService.CreateMarket(marketName, marketCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            db.Dispose();
        }

        [Fact]
        public async Task CreateMarket_ShouldNotCreateMarketIfCodeNotCorrect()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var wrongMarketCode = Constants.Test;
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketDataIsWrong);

            // Act
            var actualResult = await dataService.CreateMarket(Constants.MarketName, wrongMarketCode);

            // Asser
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            db.Dispose();
        }
    }
}
