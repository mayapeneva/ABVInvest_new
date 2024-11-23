using ABVInvest.Common;
using ABVInvest.Common.BindingModels.Deals;
using System.Security.Claims;

namespace ABVInvest.Services.Deals
{
    public interface IDealsService
    {
        Task<IEnumerable<T>> GetUserDailyDeals<T>(ClaimsPrincipal user, DateOnly date);

        Task<ApplicationResultBase> SeedDeals(DealRowBindingModel[] deserializedDeals, DateOnly date);
    }
}
