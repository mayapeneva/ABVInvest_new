using ABVInvest.Common;
using ABVInvest.Common.BindingModels.Deals;
using System.Security.Claims;

namespace ABVInvest.Services.Deals
{
    public interface IDealsService
    {
        Task<IEnumerable<T>> GetUserDailyDealsAsync<T>(ClaimsPrincipal user, DateOnly date);

        Task<ApplicationResultBase> SeedDealsAsync(IEnumerable<DealRowBindingModel> deserializedDeals, DateOnly date);
    }
}
