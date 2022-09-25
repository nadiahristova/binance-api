using AutoMapper;
using Binance.Domain.Dtos;
using Binance.Domain.Entities;
using Binance.Domain.Settings;

namespace Binance.API.Profiles;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
        CreateMap<SymbolPrice, SymbolPriceDto>()
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol.Name));
    }
}
