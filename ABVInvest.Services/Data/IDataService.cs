using ABVInvest.Common;
using ABVInvest.Data.Models;

namespace ABVInvest.Services.Data
{
    public interface IDataService
    {
        Task<ApplicationResult<Currency>> CreateCurrency(string code);

        Task<ApplicationResult<Market>> CreateMarket(string name, string mic);

        Task<ApplicationResult<Security>> CreateSecurity(string issuerName, string ISIN, string bfbCode, string currencyCode);
    }
}
