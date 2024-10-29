using System.Security.Claims;

namespace ABVInvest.Services.Portfolio
{
    public interface IPortfoliosService
    {
        Task<IEnumerable<T>> GetUserDailyPortfolio<T>(ClaimsPrincipal user, DateOnly date);
    }
}
