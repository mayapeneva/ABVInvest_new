using ABVInvest.Common;
using ABVInvest.Common.BindingModels;
using ABVInvest.Common.Constants;
using ABVInvest.Common.Validators;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using AutoMapper;

namespace ABVInvest.Services.Data
{
    public class DataService(ApplicationDbContext db, IMapper mapper)
        : BaseService(db, mapper), IDataService
    {
        public async Task<Currency?> GetOrCreateCurrencyAsync(string currencyCode)
        {
            var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == currencyCode);
            if (currency is not null) return currency;

            var currencyResult = await this.InternalCreateCurrency(currencyCode);
            return currencyResult.IsSuccessful() ? currencyResult.Data : null;
        }

        public Task<ApplicationResult<Currency>> CreateCurrencyAsync(string code)
        {
            var result = new ApplicationResult<Currency>();
            if (this.Db.Currencies.Any(c => c.Code == code))
            {
                result.Errors.Add(Messages.Data.CurrencyExists);
                return Task.FromResult(result);
            }

            return this.InternalCreateCurrency(code);
        }

        public async Task<ApplicationResult<Market>> CreateMarketAsync(string name, string mic)
        {
            var result = new ApplicationResult<Market>();
            if (this.Db.Markets.Any(m => m.Name == name || m.MIC == mic))
            {
                result.Errors.Add(Messages.Data.MarketExists);
                return result;
            }

            var market = new Market { Name = name, MIC = mic };
            if (!ModelValidator.IsValid(market))
            {
                result.Errors.Add(Messages.Data.MarketDataIsWrong);
                return result;
            }

            try
            {
                var marketResult = await this.Db.Markets.AddAsync(market);
                result.Data = marketResult.Entity;

                await this.Db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public async Task<Security?> GetOrCreateSecurityAsync(Instrument securityInfo)
        {
            var security = this.Db.Securities.SingleOrDefault(s => s.ISIN == securityInfo.ISIN);
            if (security is not null) return security;

            var securityResult = await this.InternalCreateSecurity(this.Mapper.Map<SecurityBindingModel>(securityInfo));
            return securityResult.IsSuccessful() ? securityResult.Data : null;
        }

        public Task<ApplicationResult<Security>> CreateSecurityAsync(SecurityBindingModel securityInfo)
        {
            var result = new ApplicationResult<Security>();
            if (this.Db.Securities.Any(s => s.ISIN == securityInfo.ISIN))
            {
                result.Errors.Add(Messages.Data.SecurityExists);
                return Task.FromResult(result);
            }

            return this.InternalCreateSecurity(securityInfo);
        }

        private async Task<ApplicationResult<Currency>> InternalCreateCurrency(string code)
        {
            var result = new ApplicationResult<Currency>();

            var currency = new Currency { Code = code };
            if (!ModelValidator.IsValid(currency))
            {
                result.Errors.Add(Messages.Data.CurrencyDataIsWrong);
                return result;
            }

            try
            {
                var currencyResult = await this.Db.Currencies.AddAsync(currency);
                result.Data = currencyResult.Entity;

                await this.Db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        private async Task<ApplicationResult<Security>> InternalCreateSecurity(SecurityBindingModel securityInfo)
        {
            var result = new ApplicationResult<Security>();

            var issuer = this.Db.Issuers.SingleOrDefault(i => i.Name == securityInfo.Issuer) ?? await this.CreateIssuer(securityInfo.Issuer);
            if (issuer is null)
            {
                result.Errors.Add(Messages.Data.IssuerDataIsWrong);
                return result;
            }

            var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == securityInfo.Currency);
            if (currency is null)
            {
                var createCurrencyResult = await this.CreateCurrencyAsync(securityInfo.Currency);
                if (!createCurrencyResult.IsSuccessful())
                {
                    result.Errors = createCurrencyResult.Errors;
                    return result;
                }

                currency = createCurrencyResult.Data;
            }

            var security = new Security
            {
                IssuerId = issuer.Id,
                ISIN = securityInfo.ISIN,
                BfbCode = securityInfo.BfbCode,
                Currency = currency!
            };

            security.SetSecuritiesType();
            if (!ModelValidator.IsValid(security))
            {
                result.Errors.Add(Messages.Data.SecurityDataIsWrong);
                return result;
            }

            try
            {
                var securityResult = await this.Db.Securities.AddAsync(security);
                result.Data = securityResult.Entity;

                await this.Db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        private async Task<Issuer?> CreateIssuer(string issuerName)
        {
            var issuer = new Issuer { Name = issuerName };
            if (!ModelValidator.IsValid(issuer)) return null;

            try
            {
                var issuerResult = await this.Db.Issuers.AddAsync(issuer);
                await this.Db.SaveChangesAsync();

                return issuerResult.Entity;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
