// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using AutoMapper;

using Skoruba.Duende.IdentityServer.Admin.Api.Dtos.IdentityProvider;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

namespace Skoruba.Duende.IdentityServer.Admin.Api.Mappers;
public static class IdentityProviderApiMappers
{
    static IdentityProviderApiMappers()
    {
        Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProviderApiMapperProfile>())
            .CreateMapper();
    }

    internal static IMapper Mapper { get; }
    public static T ToIdentityProviderApiModel<T>(this object source)
    {
        return Mapper.Map<T>(source);
    }

}
public class IdentityProviderApiMapperProfile : Profile
{
    public IdentityProviderApiMapperProfile()
    {
        // map to api dto
        CreateMap<IdentityProviderDto, IdentityProviderApiDto>(MemberList.Destination)
            .ForMember(apiDto => apiDto.IdentityProviderProperties,
                opts => opts.ConvertUsing(PropertiesConverter.Converter, x => x.Properties));

        // map from api dto
        CreateMap<IdentityProviderApiDto, IdentityProviderDto>(MemberList.Destination)
            .ForMember(dto => dto.Properties,
                opts => opts.ConvertUsing(PropertiesConverter.Converter, x => x.IdentityProviderProperties));

        CreateMap<IdentityProvidersDto, IdentityProvidersApiDto>(MemberList.Destination)
            .ReverseMap();
    }

    private class PropertiesConverter :
        IValueConverter<Dictionary<int, IdentityProviderPropertyDto>, Dictionary<string, string>>,
        IValueConverter<Dictionary<string, string>, Dictionary<int, IdentityProviderPropertyDto>>
    {
        public static PropertiesConverter Converter = new ();

        public Dictionary<string, string> Convert(Dictionary<int, IdentityProviderPropertyDto> sourceMember, ResolutionContext context)
        {
            var dict = sourceMember.ToDictionary(x => x.Value.Name, dto => dto.Value.Value);
            return dict;
        }

        public Dictionary<int, IdentityProviderPropertyDto> Convert(Dictionary<string, string> sourceMember, ResolutionContext context)
        {
            var index = 0;
            var dict = sourceMember.Select(i => new IdentityProviderPropertyDto { Name = i.Key, Value = i.Value });
            return dict.ToDictionary(_ => index++, item => item);
        }
    }
}