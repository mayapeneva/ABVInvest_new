using ABVInvest.Common;
using ABVInvest.Common.BindingModels.Deals;
using ABVInvest.Common.Constants;
using ABVInvest.Common.Validators;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Data.Models.Enums;
using ABVInvest.Services.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ABVInvest.Services.Deals
{
    public class DealsService : BaseService, IDealsService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDataService dataService;

        public DealsService(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IDataService dataService,
            IMapper mapper)
            : base(db, mapper)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(dataService);

            this.userManager = userManager;
            this.dataService = dataService;
        }

        public async Task<IEnumerable<T>> GetUserDailyDeals<T>(ClaimsPrincipal user, DateOnly date)
        {
            var dbUser = await userManager.GetUserAsync(user);
            return dbUser?.Deals.SingleOrDefault(p => p.Date == date)?
                .Deals?
                .Select(d => this.Mapper.Map<T>(d)) ?? [];
        }

        public async Task<ApplicationResultBase> SeedDeals(IEnumerable<DealRowBindingModel> deserializedDeals, DateOnly date)
        {
            var result = new ApplicationResultBase();
            var changesCounter = 0;

            // Group the entries by Client and process deals for each client
            var deals = deserializedDeals.GroupBy(p => p.Client.CDNNumber);
            foreach (var deal in deals)
            {
                // Check if User exists
                var user = this.Db.ApplicationUsers.SingleOrDefault(u => u.UserName == deal.Key);
                if (user is null)
                {
                    result.Errors.Add(string.Format(Messages.User.UserDoesNotExist, deal.Key));
                    continue;
                }

                // Check if there is a DailyDealsEntity created for this User and date already
                if (this.Db.DailyDeals.Any(dd => dd.ApplicationUserId == user.Id && dd.Date == date))
                {
                    result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DailyDealsAlredyExist, deal.Key, date));
                    continue;
                }

                // Create new DailyDealsEntity
                var dbDailyDeals = new DailyDeals
                {
                    Date = date,
                    Deals = []
                };

                // Create all Deals for this User
                foreach (var dealRow in deal)
                {
                    var dealRowResult = await this.CreateDealRowForUser(dealRow, deal.Key, dbDailyDeals);
                    if (!dealRowResult.IsSuccessful()) dealRowResult.Errors.ToList().ForEach(result.Errors.Add);
                }

                // Validate dailyDeals
                if (!ModelValidator.IsValid(dbDailyDeals) || dbDailyDeals.Deals.Count == 0)
                {
                    result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DailyDealsCannotBeCreated, deal.Key, date));
                    continue;
                }

                //Add dailyDeals to user's Deals
                user.Deals.Add(dbDailyDeals);
                changesCounter += await this.Db.SaveChangesAsync();
            }

            if (changesCounter > 0 && !result.IsSuccessful())
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.UploadingSumarry, changesCounter));

            return result;
        }

        private async Task<ApplicationResultBase> CreateDealRowForUser(DealRowBindingModel dealRow, string dealKey, DailyDeals dbDailyDeals)
        {
            var result = new ApplicationResultBase();
            var securityInfo = dealRow.Instrument;

            // Get or create security
            var security = await dataService.GetOrCreateSecurity(securityInfo);
            if (security is null)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.SecurityCannotBeCreated, dealKey, securityInfo.Issuer,
                    securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency));
                return result;
            }

            // Get or create currency
            var currency = await dataService.GetOrCreateCurrency(securityInfo.Currency);
            if (currency is null)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.CurrencyCannotBeCreated, securityInfo.Currency, dealKey,
                    securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode));
                return result;
            }

            var dealData = dealRow.DealData;

            // Check if such market exists
            var market = this.Db.Markets.SingleOrDefault(m => m.MIC == dealData.StockExchangeMIC);
            if (market is null)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.MarketDoesNotExist, dealData.Operation,
                    securityInfo.ISIN, dealKey, dealData.StockExchangeMIC));
                return result;
            }

            // Parse data and create deal
            var dealResult = this.ParseDataAndCreateDeal(dealData, dealKey, security, currency, market);
            if (!dealResult.IsSuccessful())
            {
                dealResult.Errors.ToList().ForEach(result.Errors.Add);
                return result;
            };

            var dbDeal = dealResult.Data;

            // Validate the Deal and add it to the dailyDeals
            if (!ModelValidator.IsValid(dbDeal!))
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealRowCannotBeCreated,
                    dealData.Operation,
                    securityInfo.ISIN,
                    dealKey,
                    dealData.ShareCount,
                    dealData.SinglePrice));
                return result;
            }

            dbDailyDeals.Deals.Add(dbDeal!);
            return result;
        }

        private ApplicationResult<Deal> ParseDataAndCreateDeal(DealData dealData, string dealKey, Security security, Currency currency, Market market)
        {
            var result = new ApplicationResult<Deal>();

            var operation = dealData.Operation;
            if (operation != ShortConstants.Deals.Buy && operation != ShortConstants.Deals.Sell)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.TypeOfDeal, operation));
                return result;
            }
            var dealType = operation == ShortConstants.Deals.Buy ? DealType.Купува : DealType.Продава;

            var ifQuantityParsed = decimal.TryParse(dealData.ShareCount.Replace(" ", string.Empty), out var quantity);
            if (!ifQuantityParsed)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.Quantity, dealData.ShareCount));
                return result;
            }

            var ifPriceParsed = decimal.TryParse(dealData.SinglePrice.Replace(" ", string.Empty), out var price);
            if (!ifPriceParsed)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.Price, dealData.SinglePrice));
                return result;
            }

            var ifCouponParsed = decimal.TryParse(dealData.Coupon.Replace(" ", string.Empty), out var coupon);
            if (!ifCouponParsed)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.Coupon, dealData.Coupon));
                return result;
            }

            var ifTotalPriceParsed = decimal.TryParse(dealData.DealAmountInShareCurrency.Replace(" ", string.Empty), out var totalPrice);
            if (!ifTotalPriceParsed)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.CurrencyValue, dealData.DealAmountInShareCurrency));
                return result;
            }

            var ifTotalPriceInBGNParsed = decimal.TryParse(dealData.DealAmountInPaymentCurrency.Replace(" ", string.Empty), out var totalPriceInBGN);
            if (!ifTotalPriceInBGNParsed)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.BgnValue, dealData.DealAmountInPaymentCurrency));
                return result;
            }

            var ifFeeParsed = decimal.TryParse(dealData.CommissionInPaymentCurrency.Replace(" ", string.Empty), out var fee);
            if (!ifFeeParsed)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.Fee, dealData.CommissionInPaymentCurrency));
                return result;
            }

            var ifSettlementParsed = DateTime.TryParse(dealData.DeliveryDate, out DateTime settlement);
            if (!ifSettlementParsed)
            {
                result.Errors.Add(string.Format(Messages.DealsAndPortfolios.DealCannotBeRegistered, dealData.Operation,
                    security.ISIN, dealKey, ShortConstants.Deals.Settlement, dealData.DeliveryDate));
                return result;
            }

            result.Data = new Deal
            {
                DealType = dealType,
                Security = security,
                Quantity = quantity,
                Price = price,
                Coupon = coupon,
                TotalPrice = totalPrice,
                TotalPriceInBGN = totalPriceInBGN,
                Fee = fee,
                Currency = currency,
                Settlement = DateOnly.FromDateTime(settlement),
                Market = market
            };

            return result;
        }
    }
}
