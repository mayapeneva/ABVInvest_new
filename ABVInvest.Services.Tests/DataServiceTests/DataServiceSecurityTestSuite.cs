using ABVInvest.Common;
using ABVInvest.Common.BindingModels;
using ABVInvest.Common.Constants;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Data;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceSecurityTestSuite : IDisposable
    {
        private ApplicationDbContext Db;
        private IDataService DataService;

        public DataServiceSecurityTestSuite()
        {
            (DataService, Db) = TestExtensions.DataServiceSetup();
        }

        [Fact]
        public async Task CreateSecurityAsync_ShouldCreateSecurity()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };
            var expectedSecuritiesCount = 1;

            // Act
            var actualResult = await DataService.CreateSecurityAsync(securityInfo);
            var actualSecuritiesCount = Db.Securities.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(Constants.IssuerName, data.Issuer.Name);
            Assert.Equal(Constants.ISIN, data.ISIN);
            Assert.Equal(Constants.BfbCode, data.BfbCode);
            Assert.Equal(Constants.CurrencyCode, data.Currency.Code);
            Assert.Equal(expectedSecuritiesCount, actualSecuritiesCount);
        }

        [Fact]
        public async Task CreateSecurityAsync_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };
            await DataService.CreateSecurityAsync(securityInfo);
            var expectedResult = new ApplicationResult<Security>();
            expectedResult.Errors.Add(Messages.Data.SecurityExists);

            // Act
            var actualResult = await DataService.CreateSecurityAsync(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task CreateSecurityAsync_ShouldCreateIssuerIfSuchDoesNotExist()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };

            // Act
            var actualResult = await DataService.CreateSecurityAsync(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());
            Assert.NotNull(actualResult.Data);

            Assert.Contains(Db.Issuers, i => i.Name == Constants.IssuerName);
        }

        [Fact]
        public async Task CreateSecurityAsync_ShouldCreateCurrencyIfSuchDoesNotExist()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };

            // Act
            var actualResult = await DataService.CreateSecurityAsync(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());
            Assert.NotNull(actualResult.Data);

            Assert.Contains(Db.Currencies, i => i.Code == Constants.CurrencyCode);
        }

        [Theory]
        [InlineData(Constants.ISIN, Constants.Test)]
        [InlineData(Constants.Test, Constants.BfbCode)]
        public async Task CreateSecurityAsync_ShouldNotCreateSrcurityIfISINOrBFBCodeNotCorrect(string isin, string bfbCode)
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = isin, BfbCode = bfbCode, Currency = Constants.CurrencyCode };
            var expectedResult = new ApplicationResult<Security>();
            expectedResult.Errors.Add(Messages.Data.SecurityDataIsWrong);

            // Act
            var actualResult = await DataService.CreateSecurityAsync(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task GetOrCreateSecurityAsync_ShouldGetSecurityIfExists()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };
            await DataService.CreateSecurityAsync(securityInfo);

            var instrument = new Instrument { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, NewCode = Constants.BfbCode, Currency = Constants.CurrencyCode };

            // Act
            var actualResult = await DataService.GetOrCreateSecurityAsync(instrument);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(Constants.ISIN, actualResult.ISIN);
            Assert.Equal(Constants.BfbCode, actualResult.BfbCode);
        }

        [Fact]
        public async Task GetOrCreateSecurityAsync_ShouldCreateSecurityIfSuchDoesNotExist()
        {
            // Arrange
            var instrument = new Instrument { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, NewCode = Constants.BfbCode, Currency = Constants.CurrencyCode };
            var expectedSecuritiesCount = 1;

            // Act
            var actualResult = await DataService.GetOrCreateSecurityAsync(instrument);
            var actualSecuritiesCount = Db.Securities.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(Constants.ISIN, actualResult.ISIN);
            Assert.Equal(Constants.BfbCode, actualResult.BfbCode);

            Assert.Equal(expectedSecuritiesCount, actualSecuritiesCount);
            Assert.Contains(Db.Securities, c => c.ISIN == Constants.ISIN);
        }

        public void Dispose() => Db?.Dispose();
    }
}
