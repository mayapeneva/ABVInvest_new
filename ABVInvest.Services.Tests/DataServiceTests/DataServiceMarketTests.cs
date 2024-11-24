using ABVInvest.Common;
using ABVInvest.Common.Constants;
using ABVInvest.Data.Models;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceMarketTests : BaseDataServiceTests
    {
        private const string MarketName = "БФБ";
        private const string MarketCode = "XBUL";

        [Fact]
        public async Task CreateMarket_ShouldCreateMarket()
        {
            // Arrange
            var expectedMarketsCount = 1;

            // Act
            var actualResult = await DataService.CreateMarket(MarketName, MarketCode);
            var actualMarketsCount = Db.Markets.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(MarketName, data.Name);
            Assert.Equal(MarketCode, data.MIC);
            Assert.Equal(expectedMarketsCount, actualMarketsCount);
        }

        [Theory]
        [InlineData(MarketName, "test")]
        [InlineData("test", MarketCode)]
        public async Task CreateMarket_ShouldNotCreateMarketIfSuchAlreadyExists(string marketName, string marketCode)
        {
            // Arrange
            await DataService.CreateMarket(MarketName, MarketCode);
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketExists);

            // Act
            var actualResult = await DataService.CreateMarket(marketName, marketCode);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task CreateMarket_ShouldNotCreateMarketIfCodeNotCorrect()
        {
            // Arrange
            var wrongMarketCode = "XBULL";
            var expectedResult = new ApplicationResult<Market>();
            expectedResult.Errors.Add(Messages.Data.MarketDataIsWrong);

            // Act
            var actualResult = await DataService.CreateMarket(MarketName, wrongMarketCode);

            // Assert

            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }
    }
}
