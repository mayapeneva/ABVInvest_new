using ABVInvest.Common;
using ABVInvest.Data.Models;

namespace ABVInvest.Services.Balances
{
    public interface IBalancesService
    {
        ApplicationResult<T> GetUserDailyBalance<T>(ApplicationUser user, DateOnly date);

        Task<ApplicationResult<DailyBalance>> CreateBalanceForUser(ApplicationUser user, DateOnly date);
    }
}
