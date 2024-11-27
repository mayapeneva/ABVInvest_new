using ABVInvest.Common;
using ABVInvest.Common.BindingModels.Portfolios;
using System.Security.Claims;

namespace ABVInvest.Services.Portfolios
{
    public interface IPortfoliosService
    {
        Task<IEnumerable<T>> GetUserDailyPortfolioAsync<T>(ClaimsPrincipal user, DateOnly date);

        Task<ApplicationResultBase> SeedPortfoliosAsync(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateOnly date);
    }
}
