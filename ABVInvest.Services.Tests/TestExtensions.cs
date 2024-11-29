using ABVInvest.Common.BindingModels.Portfolios;
using ABVInvest.Common.Constants;
using ABVInvest.Common.Mapping;
using ABVInvest.Data;
using ABVInvest.Data.Models;
using ABVInvest.Services.Balances;
using ABVInvest.Services.Data;
using ABVInvest.Services.Portfolios;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Xml.Serialization;

namespace ABVInvest.Services.Tests
{
    public class TestExtensions
    {
        public static DateOnly Date { get; set; } = DateOnly.FromDateTime(new DateTime(2020, 12, 15));

        public static Tuple<IDataService, ApplicationDbContext> DataServiceSetup()
        {
            var db = GetDb();
            var mapper = GetMapper();
            return new Tuple<IDataService, ApplicationDbContext>(new DataService(db, mapper), db);
        }

        public static Mock<ApplicationUser> UserSetup()
        {
            var moqUser = new Mock<ApplicationUser>();
            moqUser.Setup(u => u.Portfolio).Returns([ new DailySecuritiesPerClient
            {
                Date = Date,
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
