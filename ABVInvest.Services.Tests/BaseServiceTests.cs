using ABVInvest.Common.Mapping;
using ABVInvest.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ABVInvest.Services.Tests
{
    public abstract class BaseServiceTests
    {
        protected readonly ApplicationDbContext Db;
        protected readonly IMapper Mapper;

        public BaseServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ABV").Options;
            Db = new ApplicationDbContext(options);

            var mappingProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            Mapper = new Mapper(configuration);
        }
    }
}
