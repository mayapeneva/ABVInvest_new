using ABVInvest.Common;
using ABVInvest.Common.BindingModels;
using ABVInvest.Data.Models;

namespace ABVInvest.Services.Data
{
    public interface IDataService
    {
        Task<Currency?> GetOrCreateCurrencyAsync(string currencyCode);

        Task<ApplicationResult<Currency>> CreateCurrencyAsync(string code);

        Task<ApplicationResult<Market>> CreateMarketAsync(string name, string mic);

        Task<Security?> GetOrCreateSecurityAsync(Instrument securityInfo);

        Task<ApplicationResult<Security>> CreateSecurityAsync(SecurityBindingModel securityInfo);
    }
}
