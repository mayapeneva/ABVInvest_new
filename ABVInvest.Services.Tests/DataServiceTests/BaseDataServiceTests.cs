using ABVInvest.Services.Data;

namespace ABVInvest.Services.Tests.DataServiceTests
{
    public abstract class BaseDataServiceTests : BaseServiceTests
    {
        protected readonly IDataService DataService;

        public BaseDataServiceTests()
        {
            DataService = new DataService(Db, Mapper);
        }
    }
}
