using AutoMapper;
using Contracts.Events;
using Ecoeden.Search.Api.Entities;

namespace Ecoeden.Search.Api.Mappers;

public class GenericEventMappers : Profile
{
    public GenericEventMappers()
    {
        CreateMap<ProductCreated, ProductSearchSummary>().ReverseMap();
    }
}
