using ABVInvest.Common;
using ABVInvest.Common.BindingModels;
using ABVInvest.Common.Constants;
using ABVInvest.Data.Models;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceSecurityTests : BaseDataServiceTests
    {
        private const string IssuerName = "БФБ";
        private const string ISIN = "BG1100008983";
        private const string BfbCode = "HSOF";
        private const string CurrencyCode = "USD";

        [Fact]
        public async Task CreateSecurity_ShouldCreateSecurity()
        {
            // Arrange
            var expectedSecuritiesCount = 1;

            // Act
            var securityInfo = new SecurityBindingModel { Issuer = IssuerName, ISIN = ISIN, BfbCode = BfbCode, Currency = CurrencyCode };
            var actualResult = await DataService.CreateSecurity(securityInfo);
            var actualSecuritiesCount = Db.Securities.Count();

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());

            var data = actualResult.Data;
            Assert.NotNull(data);

            Assert.Equal(IssuerName, data.Issuer.Name);
            Assert.Equal(ISIN, data.ISIN);
            Assert.Equal(BfbCode, data.BfbCode);
            Assert.Equal(CurrencyCode, data.Currency.Code);
            Assert.Equal(expectedSecuritiesCount, actualSecuritiesCount);
        }

        [Fact]
        public async Task CreateSecurity_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = IssuerName, ISIN = ISIN, BfbCode = BfbCode, Currency = CurrencyCode };
            await DataService.CreateSecurity(securityInfo);
            var expectedResult = new ApplicationResult<Security>();
            expectedResult.Errors.Add(Messages.Data.SecurityExists);

            // Act
            var actualResult = await DataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }

        [Fact]
        public async Task CreateSecurity_ShouldCreateIssuerIfSuchDoesNotExist()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = IssuerName, ISIN = ISIN, BfbCode = BfbCode, Currency = CurrencyCode };

            // Act
            var actualResult = await DataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);

            Assert.Contains(Db.Issuers, i => i.Name == IssuerName);
        }

        [Fact]
        public async Task CreateSecurity_ShouldCreateCurrencyIfSuchDoesNotExist()
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = IssuerName, ISIN = ISIN, BfbCode = BfbCode, Currency = CurrencyCode };

            // Act
            var actualResult = await DataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);

            Assert.Contains(Db.Currencies, i => i.Code == CurrencyCode);
        }

        [Theory]
        [InlineData(ISIN, "test")]
        [InlineData("test", BfbCode)]
        public async Task CreateSecurity_ShouldNotCreateSrcurityIfISINOrBFBCodeNotCorrect(string isin, string bfbCode)
        {
            // Arrange
            var securityInfo = new SecurityBindingModel { Issuer = IssuerName, ISIN = isin, BfbCode = bfbCode, Currency = CurrencyCode };
            var expectedResult = new ApplicationResult<Security>();
            expectedResult.Errors.Add(Messages.Data.SecurityExists);

            // Act
            var actualResult = await DataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);
        }
    }
}
