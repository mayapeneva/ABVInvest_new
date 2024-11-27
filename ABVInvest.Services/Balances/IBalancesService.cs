using ABVInvest.Common;
using ABVInvest.Data.Models;

namespace ABVInvest.Services.Balances
{
    public interface IBalancesService
    {
        ApplicationResult<T> GetUserDailyBalanceAsync<T>(ApplicationUser user, DateOnly date);

        Task<ApplicationResult<DailyBalance>> CreateBalanceForUserAsync(ApplicationUser user, DateOnly date);
    }
}
