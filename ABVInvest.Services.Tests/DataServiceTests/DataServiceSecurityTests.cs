using ABVInvest.Common;
using ABVInvest.Common.BindingModels;
using ABVInvest.Common.Constants;
using ABVInvest.Data.Models;
using Xunit;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public class DataServiceSecurityTests
    {
        [Fact]
        public async Task CreateSecurity_ShouldCreateSecurity()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var expectedSecuritiesCount = 1;

            // Act
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };
            var actualResult = await dataService.CreateSecurity(securityInfo);
            var actualSecuritiesCount = db.Securities.Count();

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

            db.Dispose();
        }

        [Fact]
        public async Task CreateSecurity_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };
            await dataService.CreateSecurity(securityInfo);
            var expectedResult = new ApplicationResult<Security>();
            expectedResult.Errors.Add(Messages.Data.SecurityExists);

            // Act
            var actualResult = await dataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            db.Dispose();
        }

        [Fact]
        public async Task CreateSecurity_ShouldCreateIssuerIfSuchDoesNotExist()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };

            // Act
            var actualResult = await dataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());
            Assert.NotNull(actualResult.Data);

            Assert.Contains(db.Issuers, i => i.Name == Constants.IssuerName);

            db.Dispose();
        }

        [Fact]
        public async Task CreateSecurity_ShouldCreateCurrencyIfSuchDoesNotExist()
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = Constants.ISIN, BfbCode = Constants.BfbCode, Currency = Constants.CurrencyCode };

            // Act
            var actualResult = await dataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualResult.IsSuccessful());
            Assert.NotNull(actualResult.Data);

            Assert.Contains(db.Currencies, i => i.Code == Constants.CurrencyCode);

            db.Dispose();
        }

        [Theory]
        [InlineData(Constants.ISIN, Constants.Test)]
        [InlineData(Constants.Test, Constants.BfbCode)]
        public async Task CreateSecurity_ShouldNotCreateSrcurityIfISINOrBFBCodeNotCorrect(string isin, string bfbCode)
        {
            // Arrange
            var (dataService, db) = TestExtensions.DataServiceSetup();
            var securityInfo = new SecurityBindingModel { Issuer = Constants.IssuerName, ISIN = isin, BfbCode = bfbCode, Currency = Constants.CurrencyCode };
            var expectedResult = new ApplicationResult<Security>();
            expectedResult.Errors.Add(Messages.Data.SecurityDataIsWrong);

            // Act
            var actualResult = await dataService.CreateSecurity(securityInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.False(actualResult.IsSuccessful());
            Assert.Null(actualResult.Data);
            Assert.Equal(expectedResult.Errors, actualResult.Errors);

            db.Dispose();
        }
    }
}
