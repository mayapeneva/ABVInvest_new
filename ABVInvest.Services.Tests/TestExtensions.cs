using ABVInvest.Data;
using ABVInvest.Data.Models;

namespace ABVInvest.Services.Tests
{
    public static class TestExtensions
    {
        public static void SeedUser(this ApplicationDbContext db, string userName)
        {
            db.ApplicationUsers.Add(new ApplicationUser
            {
                UserName = userName
            });

            db.SaveChanges();
        }
    }
}
