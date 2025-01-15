using ABVInvest.Common;
using ABVInvest.Data.Models;
using System.Security.Claims;

namespace ABVInvest.Services.Balances
{
    public interface IBalancesService
    {
        Task<T> GetUserDailyBalanceAsync<T>(ClaimsPrincipal user, DateOnly date);

        Task<ApplicationResultBase> CreateBalanceForUserAsync(ApplicationUser user, DateOnly date);
    }
}
