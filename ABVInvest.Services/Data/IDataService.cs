using ABVInvest.Common;
using ABVInvest.Common.BindingModels;
using ABVInvest.Common.BindingModels.Portfolios;
using ABVInvest.Data.Models;

namespace ABVInvest.Services.Data
{
    public interface IDataService
    {
        Task<Currency?> GetOrCreateCurrency(string currencyCode);

        Task<ApplicationResult<Currency>> CreateCurrency(string code);

        Task<ApplicationResult<Market>> CreateMarket(string name, string mic);

        Task<Security?> GetOrCreateSecurity(Instrument securityInfo);

        Task<ApplicationResult<Security>> CreateSecurity(SecurityBindingModel securityInfo);
    }
}
