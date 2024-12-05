using ABVInvest.Common.BindingModels;
using ABVInvest.Common.Constants;
using ABVInvest.Common.TestModels;
using ABVInvest.Common.ViewModels;
using ABVInvest.Data.Models;
using AutoMapper;
using System.Globalization;

namespace ABVInvest.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<SecuritiesPerClient, PortfolioViewModel>()
                .ForMember(dest => dest.DailySecuritiesPerClientDate, opt => opt.MapFrom(src => src.DailySecuritiesPerClient.Date.ToString(ShortConstants.Common.DateTimeParseFormat)))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.ToString("N", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture))))
                .ForMember(dest => dest.AveragePriceBuy, opt => opt.MapFrom(src => src.AveragePriceBuy.ToString("N3", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture))))
                .ForMember(dest => dest.TotalPriceBuy, opt => opt.MapFrom(src => src.TotalPriceBuy.ToString("N2", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture))))
                .ForMember(dest => dest.MarketPrice, opt => opt.MapFrom(src => src.MarketPrice.ToString("N3", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture))))
                .ForMember(dest => dest.TotalMarketPrice, opt => opt.MapFrom(src => src.TotalMarketPrice.ToString("N2", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture))))
                .ForMember(dest => dest.ProfitPercentage, opt => opt.MapFrom(src => src.ProfitPercentage.ToString("N2", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture))))
                .ForMember(dest => dest.PortfolioShare, opt => opt.MapFrom(src => src.PortfolioShare.ToString("N2", CultureInfo.CreateSpecificCulture(ShortConstants.Common.SvSeCulture))));

            this.CreateMap<SecuritiesPerClient, PortfolioTestModel>();

            this.CreateMap<Instrument, SecurityBindingModel>()
                .ForMember(dest => dest.BfbCode, opt => opt.MapFrom(src => src.NewCode));

            this.CreateMap<Deal, DealTestModel>();
        }
    }
}
