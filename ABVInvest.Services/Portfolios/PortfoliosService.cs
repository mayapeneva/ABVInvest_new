using ABVInvest.Common;
using ABVInvest.Common.BindingModels.Portfolios;
using ABVInvest.Common.Constants;
using ABVInvest.Common.Validators;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Balances;
using ABVInvest.Services.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Security.Claims;

namespace ABVInvest.Services.Portfolios
{
    public class PortfoliosService : BaseService, IPortfoliosService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IBalancesService balancesService;
        private readonly IDataService dataService;
        private readonly IMapper mapper;

        public PortfoliosService(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IBalancesService balancesService,
            IDataService dataService,
            IMapper mapper)
            : base(db)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(balancesService);
            ArgumentNullException.ThrowIfNull(dataService);
            ArgumentNullException.ThrowIfNull(mapper);

            this.userManager = userManager;
            this.balancesService = balancesService;
            this.dataService = dataService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<T>> GetUserDailyPortfolio<T>(ClaimsPrincipal user, DateOnly date)
        {
            var dbUser = await userManager.GetUserAsync(user);
            return dbUser?.Portfolio?
                .SingleOrDefault(p => p.Date == date)?
                .SecuritiesPerIssuerCollection?
                .Select(mapper.Map<T>) ?? []; ;
        }

        public async Task<ApplicationResultBase> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateOnly date)
        {
            var result = new ApplicationResultBase();

            var changesCounter = 0;

            // Group the entries by Client and process portfolios for each client
            var portfolios = deserializedPortfolios.GroupBy(p => p.Client.CDNNumber);
            foreach (var portfolio in portfolios)
            {
                // Check if User exists
                var user = this.Db.ApplicationUsers.SingleOrDefault(u => u.UserName == portfolio.Key);
                if (user is null)
                {
                    result.Errors.Add(string.Format(Messages.User.UserDoesNotExist, portfolio.Key));
                    continue;
                }

                // Create new DailySecuritiesEntity
                var dbPortfolio = new DailySecuritiesPerClient
                {
                    Date = date,
                    SecuritiesPerIssuerCollection = []
                };

                // Check if there is a DailySecuritiesEntity created for this User and date already
                if (this.Db.DailySecuritiesPerClient.Any(ds => ds.ApplicationUserId == user.Id && ds.Date == date))
                {
                    dbPortfolio = this.Db.DailySecuritiesPerClient.Single(ds => ds.ApplicationUserId == user.Id && ds.Date == date);
                }

                // Create all SecuritiesPerClient for this User
                foreach (var portfolioRow in portfolio)
                {
                    var portfolioResult = await this.CreatePortfolioRowForUser(date, user.FullName, portfolioRow, portfolio.Key, dbPortfolio);
                    if (!portfolioResult.IsSuccessful())
                    {
                        portfolioResult.Errors.ToList().ForEach(result.Errors.Add);
                    }
                }

                // Validate portfolio and add it to user's Portfolios
                if (!ModelValidator.IsValid(dbPortfolio) || dbPortfolio.SecuritiesPerIssuerCollection.Count == 0)
                {
                    result.Errors.Add(string.Format(Messages.Portfolios.PortfolioCannotBeCreated, portfolio.Key, date));
                    continue;
                }

                user.Portfolio.Add(dbPortfolio);
                var dbResult = await this.Db.SaveChangesAsync();
                if (dbResult > 0)
                {
                    await balancesService.CreateBalanceForUser(user, date);
                    changesCounter += dbResult;
                }
            }

            if (changesCounter > 0 && !result.IsSuccessful())
                result.Errors.Add(string.Format(Messages.Portfolios.UploadingSumarry, changesCounter));

            return result;
        }

        private async Task<ApplicationResultBase> CreatePortfolioRowForUser(DateOnly date, string? userName, PortfolioRowBindingModel portfolioRow,
            string portfolioKey, DailySecuritiesPerClient dbPortfolio)
        {
            var result = new ApplicationResultBase();

            // Fill in user's FullName if empty
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = portfolioRow.Client.Name;
            }

            var securityInfo = portfolioRow.Instrument;

            // Get or create security
            var security = await dataService.GetOrCreateSecurity(securityInfo);
            if (security is null)
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeCreated, portfolioKey, securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency));
                return result;
            }

            // Check if such portfolioRow already exists in the usersPortfolio for this date
            if (dbPortfolio.SecuritiesPerIssuerCollection.Any(sc => sc.Security.ISIN == security.ISIN))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityExistsInThisPortfolio, portfolioKey, date, securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency));
                return result;
            }

            // Get or create currency
            var currency = await dataService.GetOrCreateCurrency(securityInfo.Currency);
            if (currency is null)
            {
                result.Errors.Add(string.Format(Messages.Portfolios.CurrencyCannotBeCreated, securityInfo.Currency, portfolioKey, securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode));
                return result;
            }

            var accountData = portfolioRow.AccountData;

            // Parse data and create SecuritiesPerClient
            var securitiesResult = this.ParseDataAndCreateSecuritiesPerClient(portfolioRow, portfolioKey, accountData, security, currency);
            if (!securitiesResult.IsSuccessful())
            {
                securitiesResult.Errors.ToList().ForEach(result.Errors.Add);
                return result;
            }

            var securitiesPerClient = securitiesResult.Data;

            // Validate SecuritiesPerClient and add them to the portfolio
            if (!ModelValidator.IsValid(securitiesPerClient!))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeCreated, portfolioKey, securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency));
                return result;
            }

            dbPortfolio.SecuritiesPerIssuerCollection.Add(securitiesPerClient!);
            return result;
        }

        private ApplicationResult<SecuritiesPerClient> ParseDataAndCreateSecuritiesPerClient(PortfolioRowBindingModel portfolioRow, string portfolioKey, AccountData accountData, Security security, Currency currency)
        {
            var result = new ApplicationResult<SecuritiesPerClient>();
            if (!decimal.TryParse(accountData.Quantity.Replace(" ", string.Empty), out var quantity))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, ShortConstants.Portfolios.Quantity, accountData.Quantity));
                return result;
            }

            if (!decimal.TryParse(accountData.OpenPrice.Replace(" ", string.Empty), out var averagePriceBuy))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, ShortConstants.Portfolios.AveragePrice, accountData.OpenPrice));
                return result;
            }

            if (!decimal.TryParse(accountData.MarketPrice.Replace(" ", string.Empty), out var marketPrice))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, ShortConstants.Portfolios.MarketPrice, accountData.MarketPrice));
                return result;
            }

            if (!decimal.TryParse(accountData.MarketValue.Replace(" ", string.Empty), out var totalMarketPrice))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, ShortConstants.Portfolios.MarketValue, accountData.MarketValue));
                return result;
            }

            if (!decimal.TryParse(accountData.Result.Replace(" ", string.Empty), out var profit))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, ShortConstants.Portfolios.Profit, accountData.Result));
                return result;
            }

            if (!decimal.TryParse(accountData.ResultBGN.Replace(" ", string.Empty), out var profitInBGN))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, profitInBGN.ToString("N2", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture)), accountData.ResultBGN));
                return result;
            }

            if (!decimal.TryParse(portfolioRow.Other.YieldPercent.Replace(" ", string.Empty), out var profitPercent))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, ShortConstants.Portfolios.ProfitInPersentage, portfolioRow.Other.YieldPercent));
                return result;
            }

            if (!decimal.TryParse(portfolioRow.Other.RelativePart.Replace(" ", string.Empty), out var portfolioShare))
            {
                result.Errors.Add(string.Format(Messages.Portfolios.SecurityCannotBeRegistered, portfolioKey, ShortConstants.Portfolios.PortfolioShare, portfolioRow.Other.RelativePart));
                return result;
            }

            result.Data = new SecuritiesPerClient
            {
                Security = security,
                Quantity = quantity,
                Currency = currency,
                AveragePriceBuy = averagePriceBuy,
                MarketPrice = marketPrice,
                TotalMarketPrice = totalMarketPrice,
                Profit = profit,
                ProfitInBGN = profitInBGN,
                ProfitPercentage = profitPercent,
                PortfolioShare = portfolioShare
            };

            return result;
        }
    }
}
