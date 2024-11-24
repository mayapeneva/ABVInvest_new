using ABVInvest.Data;
using AutoMapper;

namespace ABVInvest.Services
{
    public abstract class BaseService
    {
        protected BaseService(ApplicationDbContext db, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(mapper);

            this.Db = db;
            this.Mapper = mapper;
        }

        protected ApplicationDbContext Db { get; set; }

        protected IMapper Mapper { get; set; }
    }
}
