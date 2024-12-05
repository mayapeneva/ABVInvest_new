using ABVInvest.Common.BindingModels.Deals;
using ABVInvest.Common.BindingModels.Portfolios;
using ABVInvest.Common.Constants;
using ABVInvest.Common.Mapping;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Data.Models.Enums;
using ABVInvest.Services.Balances;
using ABVInvest.Services.Data;
using ABVInvest.Services.Deals;
using ABVInvest.Services.Portfolios;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Xml.Serialization;

namespace ABVInvest.Services.Tests
{
    public class TestHelper
    {
        public static DateOnly PortfoliosDate { get; set; } = DateOnly.FromDateTime(new DateTime(2020, 12, 15));
        public static DateOnly DealsDate { get; set; } = DateOnly.FromDateTime(new DateTime(2020, 12, 16));

        public static Tuple<IDataService, ApplicationDbContext> DataServiceSetup()
        {
            var db = GetDb();
            var mapper = GetMapper();
            return new Tuple<IDataService, ApplicationDbContext>(new DataService(db, mapper), db);
        }

        public static Mock<ApplicationUser> UserSetupForPortfoliosTests()
        {
            var moqUser = new Mock<ApplicationUser>();
            moqUser.Setup(u => u.Portfolio).Returns([ new DailySecuritiesPerClient
            {
                Date = PortfoliosDate,
                SecuritiesPerIssuerCollection = [ new SecuritiesPerClient
                    {
                        Quantity = 100,
                        AveragePriceBuy = 100,
                        MarketPrice = 200,
                        TotalMarketPrice = 20000,
                        Profit = 10000,
                        ProfitInBGN = 10000,
                        ProfitPercentage = 100,
                        PortfolioShare = 10
                    }
                ]
            }]);

            return moqUser;
        }

        public static Mock<ApplicationUser> UserSetupForDealsTests()
        {
            var moqUser = new Mock<ApplicationUser>();
            moqUser.Setup(u => u.Deals).Returns([ new DailyDeals
            {
                Date = DealsDate,
                Deals = [ new Deal
                    {
                        DealType = DealType.Купува,
                        Quantity = 100,
                        Price = 100,
                        Coupon = 0,
                        TotalPrice = 10000,
                        Fee = 90,
                        Settlement = DateOnly.FromDateTime(new DateTime(2020, 12, 18))
                    }
                ]
            }]);

            return moqUser;
        }

        public static Tuple<IPortfoliosService, ApplicationDbContext> PortfoliosServiceSetup(ClaimsPrincipal principal, Mock<ApplicationUser> moqUser)
        {
            var db = GetDb();
            var mapper = GetMapper();

            var dataService = new DataService(db, mapper);
            var balancesService = new BalancesService(db, mapper);

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(um => um.GetUserAsync(principal)).Returns(Task.FromResult(moqUser?.Object));

            var portfoliosService = new PortfoliosService(db, userManager.Object, balancesService, dataService, mapper);
            return new Tuple<IPortfoliosService, ApplicationDbContext>(portfoliosService, db);
        }

        public static async Task<IEnumerable<PortfolioRowBindingModel>> DeserialisePortfolios(string fileName)
        {
            var xmlFileContentString = File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute(ShortConstants.Common.XmlRootAttr));
            var deserializedPortfolios = serializer.Deserialize(new StringReader(xmlFileContentString)) as PortfolioRowBindingModel[];

            return deserializedPortfolios ?? [];
        }

        public static Tuple<IDealsService, ApplicationDbContext> DealsServiceSetup(ClaimsPrincipal principal, Mock<ApplicationUser> moqUser)
        {
            var db = GetDb();
            var mapper = GetMapper();

            var dataService = new DataService(db, mapper);

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(um => um.GetUserAsync(principal)).Returns(Task.FromResult(moqUser?.Object));

            var dealsService = new DealsService(db, userManager.Object, dataService, mapper);
            return new Tuple<IDealsService, ApplicationDbContext>(dealsService, db);
        }

        public static async Task<IEnumerable<DealRowBindingModel>> DeserialiseDeals(string fileName)
        {
            var xmlFileContentString = File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute(ShortConstants.Common.XmlRootAttr));
            var deserializedDeals = serializer.Deserialize(new StringReader(xmlFileContentString)) as DealRowBindingModel[];

            return deserializedDeals ?? [];
        }

        private static ApplicationDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var db = new ApplicationDbContext(options);
            return db;
        }

        private static Mapper GetMapper()
        {
            var mappingProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            var mapper = new Mapper(configuration);
            return mapper;
        }
    }
}
