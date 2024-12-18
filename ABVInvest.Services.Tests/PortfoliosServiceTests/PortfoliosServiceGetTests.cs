﻿using ABVInvest.Common.TestModels;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Portfolios;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ABVInvest.Services.Tests.PortfoliosServiceTests
{
    public class PortfoliosServiceGetTests : IDisposable
    {
        private readonly ApplicationDbContext Db;
        private readonly IPortfoliosService PortfoliosService;
        private readonly Mock<ApplicationUser> MoqUser;
        private readonly ClaimsPrincipal Principal;

        public PortfoliosServiceGetTests()
        {
            MoqUser = TestHelper.UserSetupForPortfoliosTests();
            Principal = new ClaimsPrincipal();
            (PortfoliosService, Db) = TestHelper.PortfoliosServiceSetup(Principal, MoqUser);
        }

        [Fact]
        public async Task GetUserDailyPortfolioAsync_ShouldReturnDailyPortfolioWithCorrectTotalMarketPrice()
        {
            // Arange
            var expectedTotalMarketPrice = MoqUser.Object.Portfolio.Select(p => p.SecuritiesPerIssuerCollection.Sum(s => s.TotalMarketPrice));

            // Act
            var actualResult = await PortfoliosService.GetUserDailyPortfolioAsync<PortfolioTestModel>(Principal, TestHelper.PortfoliosDate);
            var actualTotalMarketPrice = actualResult.Select(p => p.TotalMarketPrice);

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
            Assert.Equal(expectedTotalMarketPrice, actualTotalMarketPrice);
        }

        [Fact]
        public async Task GetUserDailyPortfolioAsync_ShouldReturnEmptyCollectionIfThereIsNoPortfolioForThisDate()
        {
            // Arange
            var date = DateOnly.FromDateTime(new DateTime(2020, 12, 27));

            // Act
            var actualResult = await PortfoliosService.GetUserDailyPortfolioAsync<PortfolioTestModel>(Principal, date);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task GetUserDailyPortfolioAsync_ShouldReturnEmptyCollectionIfThereIsNoSuchUser()
        {
            // Arange
            var user = new ClaimsPrincipal();

            // Act
            var actualResult = await PortfoliosService.GetUserDailyPortfolioAsync<PortfolioTestModel>(user, TestHelper.PortfoliosDate);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Empty(actualResult);
        }

        public void Dispose() => Db?.Dispose();
    }
}
