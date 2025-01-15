using ABVInvest.Common.TestModels;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Balances;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ABVInvest.Services.Tests.BalancesServiceTests
{
    public class BalancesServiceTests
    {
        private readonly ApplicationDbContext Db;
        private readonly IBalancesService BalanacesService;

        private readonly Mock<ApplicationUser> MoqUser;
        private readonly ClaimsPrincipal Principal;

        public BalancesServiceTests()
        {
            MoqUser = TestHelper.UserSetupForBalancesTests();
            Principal = new ClaimsPrincipal();
            (BalanacesService, Db) = TestHelper.BalancesServiceSetup(Principal, MoqUser);
        }

        [Fact]
        public async Task CreateBalanceForUserAsync_ShouldNotCreateSecondDailyBalanceForUserForSameDate()
        {
            // Arrange
            var firstResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);
            Assert.True(firstResult.IsSuccessful());

            var expectedUserBalancesCount = 1;

            // Act
            var secondResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);
            Assert.True(secondResult.IsSuccessful());

            var actualUserBalancesCount = MoqUser.Object.Balances.Count(b => b.Date == TestHelper.BalancesDate);

            // Assert
            Assert.Equal(expectedUserBalancesCount, actualUserBalancesCount);
        }

        [Fact]
        public async Task CreateBalanceForUserAsync_ShouldCreateDailyBalanceForUser()
        {
            // Arrange
            var expectedUserBalancesCount = 1;

            // Act
            var actualResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);
            var actualUserBalancesCount = MoqUser.Object.Balances.Count;

            // Assert
            Assert.True(actualResult.IsSuccessful());
            Assert.Equal(expectedUserBalancesCount, actualUserBalancesCount);
            Assert.Contains(MoqUser.Object.Balances, b => b.Date == TestHelper.BalancesDate);
        }

        [Fact]
        public async Task CreateBalanceForUserAsync_ShouldCreateBalanceForUser()
        {
            // Act
            var actualResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);

            // Assert
            Assert.True(actualResult.IsSuccessful());
            Assert.NotNull(MoqUser.Object.Balances.SingleOrDefault(b => b.Date == TestHelper.BalancesDate)?.Balance);
        }

        [Fact]
        public async Task CreateBalanceForUserAsync_ShouldReturnBalanceWithCorrectVirtualProfit()
        {
            // Arrange
            var expectedVirtualProfit = 10000;

            // Act
            var actualResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);
            var balance = MoqUser.Object.Balances.SingleOrDefault(b => b.Date == TestHelper.BalancesDate)?.Balance;
            var actualVirtualProfit = balance?.VirtualProfit;

            // Assert
            Assert.True(actualResult.IsSuccessful());
            Assert.Equal(expectedVirtualProfit, actualVirtualProfit);
        }

        [Fact]
        public async Task CreateBalanceForUserAsync_ShouldReturnBalanceWithCorrectVirtualProfitPercentage()
        {
            // Arrange
            var expectedVirtualProfitPercentage = 100;

            // Act
            var actualResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);
            var balance = MoqUser.Object.Balances.SingleOrDefault(b => b.Date == TestHelper.BalancesDate)?.Balance;
            var actualVirtualProfitPercentage = balance?.VirtualProfitPercentage;

            // Assert
            Assert.True(actualResult.IsSuccessful());
            Assert.Equal(expectedVirtualProfitPercentage, actualVirtualProfitPercentage);
        }

        [Fact]
        public async Task GetUserDailyBalanceAsync_ShouldReturnBalance()
        {
            // Arrange
            var createResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);

            // Act
            var actualUserBalance = await BalanacesService.GetUserDailyBalanceAsync<BalanceTestModel>(Principal, TestHelper.BalancesDate);

            // Assert
            Assert.True(createResult.IsSuccessful());
            Assert.NotNull(actualUserBalance);
        }

        [Fact]
        public async Task GetUserDailyBalanceAsync_ShouldReturnBalanceWithCorrectProfitFigures()
        {
            // Arrange
            var createResult = await BalanacesService.CreateBalanceForUserAsync(MoqUser.Object, TestHelper.BalancesDate);
            var expectedUserBalance = new BalanceTestModel
            {
                VirtualProfit = 10000,
                VirtualProfitPercentage = 100,
            };

            // Act
            var actualUserBalance = await BalanacesService.GetUserDailyBalanceAsync<BalanceTestModel>(Principal, TestHelper.BalancesDate);

            // Assert
            Assert.True(createResult.IsSuccessful());
            Assert.NotNull(actualUserBalance);
            Assert.Equal(expectedUserBalance.VirtualProfit, actualUserBalance?.VirtualProfit);
            Assert.Equal(expectedUserBalance.VirtualProfitPercentage, actualUserBalance?.VirtualProfitPercentage);
        }

        [Fact]
        public async Task GetUserDailyBalanceAsync_ShouldNotReturnBalanceIfSuchDoesNotExist()
        {
            // Act
            var actualUserBalance = await BalanacesService.GetUserDailyBalanceAsync<BalanceTestModel>(Principal, TestHelper.BalancesDate);

            // Assert
            Assert.Null(actualUserBalance);
        }

        [Fact]
        public async Task GetUserDailyBalanceAsync_ShouldReturnEmptyCollectionIfThereIsNoSuchUser()
        {
            // Arange
            var user = new ClaimsPrincipal();

            // Act
            var actualUserBalance = await BalanacesService.GetUserDailyBalanceAsync<BalanceTestModel>(user, TestHelper.BalancesDate);

            // Assert
            Assert.Null(actualUserBalance);
        }

        public void Dispose() => Db?.Dispose();
    }
}
