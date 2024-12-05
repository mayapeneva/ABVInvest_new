using ABVInvest.Common.TestModels;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Deals;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ABVInvest.Services.Tests.DealsServiceTests
{
    public class DealsServiceGetTests : IDisposable
    {
        private readonly ApplicationDbContext Db;
        private readonly IDealsService DealsService;
        private readonly Mock<ApplicationUser> MoqUser;
        private readonly ClaimsPrincipal Principal;

        public DealsServiceGetTests()
        {
            MoqUser = TestExtensions.UserSetupForDealsTests();
            Principal = new ClaimsPrincipal();
            (DealsService, Db) = TestExtensions.DealsServiceSetup(Principal, MoqUser);
        }

        [Fact]
        public async Task GetUserDailyDeals_ShouldReturnDailyDealsWithCorrectTotalPrice()
        {
            // Arange
            var expectedTotalPrice = MoqUser.Object.Deals.Select(dd => dd.Deals.Sum(d => d.TotalPrice));

            // Act
            var actualResult = await DealsService.GetUserDailyDealsAsync<DealTestModel>(Principal, TestExtensions.DealsDate);
            var actualTotalPrice = actualResult.Select(d => d.TotalPrice);

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
            Assert.Equal(expectedTotalPrice, actualTotalPrice);
        }

        [Fact]
        public async Task GetUserDailyDeals_ShouldReturnNullIfThereIsNoDealsForThisDate()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 27));

            // Act
            var actualResult = await DealsService.GetUserDailyDealsAsync<DealTestModel>(Principal, date);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task GetUserDailyDeals_ShouldReturnNullIfThereIsNoSuchUser()
        {
            // Arange
            var user = new ClaimsPrincipal();

            // Act
            var actualResult = await DealsService.GetUserDailyDealsAsync<DealTestModel>(user, TestExtensions.DealsDate);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Empty(actualResult);
        }

        public void Dispose() => Db?.Dispose();
    }
}
