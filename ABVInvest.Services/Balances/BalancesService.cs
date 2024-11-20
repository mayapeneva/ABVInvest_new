using ABVInvest.Common;
using ABVInvest.Common.Constants;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using AutoMapper;

namespace ABVInvest.Services.Balances
{
    public class BalancesService : BaseService, IBalancesService
    {
        private readonly IMapper mapper;

        public BalancesService(ApplicationDbContext db, IMapper mapper)
            : base(db)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            this.mapper = mapper;
        }

        public ApplicationResult<T> GetUserDailyBalance<T>(ApplicationUser user, DateOnly date)
        {
            var result = new ApplicationResult<T>();

            var dailyBalance = user.Balances.SingleOrDefault(b => b.Date == date);
            if (dailyBalance is null)
            {
                result.Errors.Add(string.Format(Messages.Data.NoBalance, DateTime.UtcNow.ToString(ShortConstants.Common.DateTimeParseFormat)));
                return result;
            }

            result.Data = mapper.Map<T>(dailyBalance.Balance);
            return result;
        }

        public async Task<ApplicationResult<DailyBalance>> CreateBalanceForUser(ApplicationUser user, DateOnly date)
        {
            var result = new ApplicationResult<DailyBalance>();

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

            result.Data = dailyBalance;
            return result;
        }
    }
}
