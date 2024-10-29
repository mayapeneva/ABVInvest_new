using ABVInvest.Data;

namespace ABVInvest.Services
{
    public abstract class BaseService
    {
        protected BaseService(ApplicationDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);
            this.Db = db;
        }

        protected ApplicationDbContext Db { get; set; }
    }
}
