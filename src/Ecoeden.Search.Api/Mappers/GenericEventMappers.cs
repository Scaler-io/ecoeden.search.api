using AutoMapper;
using Contracts.Events;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.EventBus.Contracts;

namespace Ecoeden.Search.Api.Mappers;

public class GenericEventMappers : Profile
{
    public GenericEventMappers()
    {
        CreateMap<ProductCreated, ProductSearchSummary>().ReverseMap();
        CreateMap<ProductUpdated, ProductSearchSummary>().ReverseMap();
        CreateMap<UserCreated, UserSearchSummary>().ReverseMap();
        CreateMap<UserUpdated, UserSearchSummary>().ReverseMap();
    }
}
