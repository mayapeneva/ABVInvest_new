using ABVInvest.Common;
using ABVInvest.Common.BindingModels.Portfolios;
using System.Security.Claims;

namespace ABVInvest.Services.Portfolios
{
    public interface IPortfoliosService
    {
        Task<IEnumerable<T>> GetUserDailyPortfolio<T>(ClaimsPrincipal user, DateOnly date);

        Task<ApplicationResultBase> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateOnly date);
    }
}
