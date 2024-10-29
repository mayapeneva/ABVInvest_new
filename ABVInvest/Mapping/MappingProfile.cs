using ABVInvest.Common;
using ABVInvest.Common.ViewModels;
using ABVInvest.Data.Models;
using AutoMapper;
using System.Globalization;

namespace ABVInvest.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SecuritiesPerClient, PortfolioViewModel>()
                .ForMember(dest => dest.DailySecuritiesPerClientDate, opt => opt.MapFrom(src => src.DailySecuritiesPerClient.Date.ToString(Constants.ViewModel.DateTimeParseFormat)))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.ToString("N", CultureInfo.CreateSpecificCulture(Constants.ViewModel.SvSeCulture))))
                .ForMember(dest => dest.AveragePriceBuy, opt => opt.MapFrom(src => src.AveragePriceBuy.ToString("N3", CultureInfo.CreateSpecificCulture(Constants.ViewModel.SvSeCulture))))
                .ForMember(dest => dest.TotalPriceBuy, opt => opt.MapFrom(src => src.TotalPriceBuy.ToString("N2", CultureInfo.CreateSpecificCulture(Constants.ViewModel.SvSeCulture))))
                .ForMember(dest => dest.MarketPrice, opt => opt.MapFrom(src => src.MarketPrice.ToString("N3", CultureInfo.CreateSpecificCulture(Constants.ViewModel.SvSeCulture))))
                .ForMember(dest => dest.TotalMarketPrice, opt => opt.MapFrom(src => src.TotalMarketPrice.ToString("N2", CultureInfo.CreateSpecificCulture(Constants.ViewModel.SvSeCulture))))
                .ForMember(dest => dest.ProfitPercentage, opt => opt.MapFrom(src => src.ProfitPercentage.ToString("N2", CultureInfo.CreateSpecificCulture(Constants.ViewModel.SvSeCulture))))
                .ForMember(dest => dest.PortfolioShare, opt => opt.MapFrom(src => src.PortfolioShare.ToString("N2", CultureInfo.CreateSpecificCulture(Constants.ViewModel.SvSeCulture))));
        }
    }
}
