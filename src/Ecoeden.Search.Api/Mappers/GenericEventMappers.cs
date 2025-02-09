﻿using AutoMapper;
using Contracts.Events;
using Ecoeden.Search.Api.Entities;

namespace Ecoeden.Search.Api.Mappers;

public class GenericEventMappers : Profile
{
    public GenericEventMappers()
    {
        CreateMap<ProductCreated, ProductSearchSummary>().ReverseMap();
        CreateMap<ProductUpdated, ProductSearchSummary>().ReverseMap();
        CreateMap<UserCreated, UserSearchSummary>().ReverseMap();
        CreateMap<UserUpdated, UserSearchSummary>().ReverseMap();
        CreateMap<SupplierCreated, SupplierSearchSummary>()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.LastUpdatedOn))
            .ReverseMap();
        CreateMap<SupplierUpdated, SupplierSearchSummary>()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.LastUpdatedOn))
            .ReverseMap();
        CreateMap<CustomerCreated, CustomerSearchSummary>()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.LastUpdatedOn))
            .ReverseMap();
        CreateMap<CustomerUpdated, CustomerSearchSummary>()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.LastUpdatedOn))
            .ReverseMap();
        CreateMap<UnitCreated, UnitSearchSummary>()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.LastUpdatedOn))
            .ReverseMap();
        CreateMap<UnitUpdated, UnitSearchSummary>()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.LastUpdatedOn))
            .ReverseMap();
        CreateMap<UnitDeleted, UnitSearchSummary>()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.LastUpdatedOn))
            .ReverseMap();
    }
}
