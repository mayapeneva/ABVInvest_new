using ABVInvest.Common;
using ABVInvest.Common.Constants;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Data;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceMarketTestSuite : IDisposable
    {
        private ApplicationDbContext Db;
        private IDataService DataService;

        public DataServiceMarketTestSuite()
        {
            (DataService, Db) = TestExtensions.DataServiceSetup();
        }

        [Fact]
        public async Task CreateMarketAsync_ShouldCreateMarket()
        {
            // Arrange
            var expectedMarketsCount = 1;

            // Act
            var actualResult = await DataService.CreateMarketAsync(Constants.MarketName, Constants.MarketCode);
            var actualMarketsCount = Db.Markets.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(Constants.MarketName, data.Name);
            Assert.Equal(Constants.MarketCode, data.MIC);
            Assert.Equal(expectedMarketsCount, actualMarketsCount);
        }

        [Theory]
        [InlineData(Constants.MarketName, Constants.Test)]
        [InlineData(Constants.Test, Constants.MarketCode)]
        public async Task CreateMarketAsync_ShouldNotCreateMarketIfSuchAlreadyExists(string marketName, string marketCode)
        {
            // Arrange
            await DataService.CreateMarketAsync(Constants.MarketName, Constants.MarketCode);
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketExists);

            // Act
            var actualResult = await DataService.CreateMarketAsync(marketName, marketCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task CreateMarketAsync_ShouldNotCreateMarketIfCodeNotCorrect()
        {
            // Arrange
            var wrongMarketCode = Constants.Test;
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketDataIsWrong);

            // Act
            var actualResult = await DataService.CreateMarketAsync(Constants.MarketName, wrongMarketCode);

            // Asser
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        public void Dispose() => Db?.Dispose();
    }
}
