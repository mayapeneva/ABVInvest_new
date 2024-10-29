using ABVInvest.Data;
using ABVInvest.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ABVInvest.Services.Portfolio
{
    public class PortfoliosService : IPortfoliosService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public PortfoliosService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(userManager);
            ArgumentNullException.ThrowIfNull(mapper);

            this.userManager = userManager;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<T>> GetUserDailyPortfolio<T>(ClaimsPrincipal user, DateOnly date)
        {
            var dbUser = await this.userManager.GetUserAsync(user);
            return dbUser?.Portfolio?
                .SingleOrDefault(p => p.Date == date)?
                .SecuritiesPerIssuerCollection?
                .Select(mapper.Map<T>) ?? []; ;
        }
    }
}
