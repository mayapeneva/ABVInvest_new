using ABVInvest.Common.Mapping;
using ABVInvest.Data;
using ABVInvest.Services.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ABVInvest.Services.Tests
{
    public class TestExtensions
    {
        public static Tuple<IDataService, ApplicationDbContext> DataServiceSetup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var db = new ApplicationDbContext(options);

            var mappingProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            var mapper = new Mapper(configuration);

            return new Tuple<IDataService, ApplicationDbContext>(new DataService(db, mapper), db);
        }
    }
}
