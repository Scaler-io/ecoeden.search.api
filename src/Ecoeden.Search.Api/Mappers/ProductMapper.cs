using AutoMapper;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Contracts;

namespace Ecoeden.Search.Api.Mappers;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<Product, ProductSearchSummary>()
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => DateTime.Parse(s.MetaData.CreatedAt)))
            .ForMember(d => d.LastUpdatedOn, o => o.MapFrom(s => DateTime.Parse(s.MetaData.UpdatedAt)));
    }
}
