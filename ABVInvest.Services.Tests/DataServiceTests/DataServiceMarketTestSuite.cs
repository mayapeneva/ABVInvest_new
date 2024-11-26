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
        private IDataService DataService;
        private ApplicationDbContext Db;

        public DataServiceMarketTestSuite()
        {
            (DataService, Db) = TestExtensions.DataServiceSetup();
        }

        [Fact]
        public async Task CreateMarket_ShouldCreateMarket()
        {
            // Arrange
            var expectedMarketsCount = 1;

            // Act
            var actualResult = await DataService.CreateMarket(Constants.MarketName, Constants.MarketCode);
            var actualMarketsCount = Db.Markets.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(Constants.MarketName, data.Name);
            Assert.Equal(Constants.MarketCode, data.MIC);
            Assert.Equal(expectedMarketsCount, actualMarketsCount);

            Db.Dispose();
        }

        [Theory]
        [InlineData(Constants.MarketName, Constants.Test)]
        [InlineData(Constants.Test, Constants.MarketCode)]
        public async Task CreateMarket_ShouldNotCreateMarketIfSuchAlreadyExists(string marketName, string marketCode)
        {
            // Arrange
            await DataService.CreateMarket(Constants.MarketName, Constants.MarketCode);
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketExists);

            // Act
            var actualResult = await DataService.CreateMarket(marketName, marketCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            Db.Dispose();
        }

        [Fact]
        public async Task CreateMarket_ShouldNotCreateMarketIfCodeNotCorrect()
        {
            // Arrange
            var wrongMarketCode = Constants.Test;
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketDataIsWrong);

            // Act
            var actualResult = await DataService.CreateMarket(Constants.MarketName, wrongMarketCode);

            // Asser
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            Db.Dispose();
        }

        public void Dispose() => Db?.Dispose();
    }
}
