using ABVInvest.Common;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ABVInvest.Services.Balances
{
    public class BalancesService : BaseService, IBalancesService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public BalancesService(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
            : base(db, mapper)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            this.userManager = userManager;
        }

        public async Task<T> GetUserDailyBalanceAsync<T>(ClaimsPrincipal user, DateOnly date)
        {
            var dbUser = await userManager.GetUserAsync(user);
            return this.Mapper.Map<T>(dbUser?.Balances.SingleOrDefault(b => b.Date == date)?.Balance);
        }

        public async Task<ApplicationResultBase> CreateBalanceForUserAsync(ApplicationUser user, DateOnly date)
        {
            var result = new ApplicationResultBase();

            var hasUserABalanceForTheDate = user.Balances.Any(b => b.Date == date);
            var dailyBalance =
                hasUserABalanceForTheDate
                    ? user.Balances.Single(b => b.Date == date)
                    : new DailyBalance
                    {
                        Date = date,
                        Balance = new Balance()
                    };

            try
            {
                dailyBalance.Balance.SetBalanceFigures(user, date);
                if (!hasUserABalanceForTheDate) user.Balances.Add(dailyBalance);

                await this.Db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
                return result;
            }

            return result;
        }
    }
}
