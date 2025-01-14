﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Duende.IdentityServer.EntityFramework.Options;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Skoruba.AuditLogging.Services;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Mappers;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Resources;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.Duende.IdentityServer.Admin.UnitTests.Mocks;

using Xunit;

namespace Skoruba.Duende.IdentityServer.Admin.UnitTests.Services;

public class ApiResourceServiceTests
{
    private static IClientRepository GetClientRepository(IdentityServerConfigurationDbContext context)
    {
        IClientRepository clientRepository = new ClientRepository<IdentityServerConfigurationDbContext>(context);

        return clientRepository;
    }

    private static IApiResourceRepository GetApiResourceRepository(IdentityServerConfigurationDbContext context)
    {
        IApiResourceRepository apiResourceRepository = new ApiResourceRepository<IdentityServerConfigurationDbContext>(context);

        return apiResourceRepository;
    }

    private static IClientService GetClientService(IClientRepository repository, IClientServiceResources resources, IAuditEventLogger auditEventLogger)
    {
        IClientService clientService = new ClientService(repository, resources, auditEventLogger);

        return clientService;
    }

    private static IApiResourceService GetApiResourceService(IApiResourceRepository repository, IApiResourceServiceResources resources, IClientService clientService, IAuditEventLogger auditEventLogger)
    {
        IApiResourceService apiResourceService = new ApiResourceService(repository, resources, clientService, auditEventLogger);

        return apiResourceService;
    }

    private static IApiResourceService GetApiResourceService(IdentityServerConfigurationDbContext context)
    {
        var apiResourceRepository = GetApiResourceRepository(context);
        var clientRepository = GetClientRepository(context);

        var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
        var localizerApiResource = localizerApiResourceMock.Object;

        var localizerClientResourceMock = new Mock<IClientServiceResources>();
        var localizerClientResource = localizerClientResourceMock.Object;

        var auditLoggerMock = new Mock<IAuditEventLogger>();
        var auditLogger = auditLoggerMock.Object;

        var clientService = GetClientService(clientRepository, localizerClientResource, auditLogger);
        var apiResourceService = GetApiResourceService(apiResourceRepository, localizerApiResource, clientService, auditLogger);

        return apiResourceService;
    }

    private static IdentityServerConfigurationDbContext GetDbContext()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton(new ConfigurationStoreOptions());
        serviceCollection.AddSingleton(new OperationalStoreOptions());

        serviceCollection.AddDbContext<IdentityServerConfigurationDbContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var context = serviceProvider.GetService<IdentityServerConfigurationDbContext>();

        return context;
    }

    [Fact]
    public async Task AddApiResourceAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResourceDto);

        //Get new api resource
        var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

        var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

        //Assert new api resource
        newApiResourceDto.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));
    }

    [Fact]
    public async Task GetApiResourceAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResourceDto);

        //Get new api resource
        var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

        var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

        //Assert new api resource
        newApiResourceDto.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));
    }

    [Fact]
    public async Task RemoveApiResourceAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResourceDto);

        //Get new api resource
        var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

        var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

        //Assert new api resource
        newApiResourceDto.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

        //Remove api resource
        await apiResourceService.DeleteApiResourceAsync(newApiResourceDto);

        //Try get removed api resource
        var removeApiResource = await context.ApiResources.Where(x => x.Id == apiResource.Id)
            .SingleOrDefaultAsync();

        //Assert removed api resource
        removeApiResource.Should().BeNull();
    }

    [Fact]
    public async Task UpdateApiResourceAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResourceDto);

        //Get new api resource
        var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

        var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

        //Assert new api resource
        newApiResourceDto.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

        //Detached the added item
        context.Entry(apiResource).State = EntityState.Detached;

        //Generete new api resuorce with added item id
        var updatedApiResource = ApiResourceDtoMock.GenerateRandomApiResource(apiResource.Id);

        //Update api resource
        await apiResourceService.UpdateApiResourceAsync(updatedApiResource);

        var updatedApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

        //Assert updated api resuorce
        updatedApiResourceDto.Should().BeEquivalentTo(updatedApiResource, options => options.Excluding(o => o.Id));
    }

    [Fact]
    public async Task AddApiSecretAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResourceDto);

        //Get new api resource
        var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

        var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

        //Assert new api resource
        newApiResourceDto.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

        //Generate random new api secret
        var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

        //Add new api secret
        await apiResourceService.AddApiSecretAsync(apiSecretsDto);

        //Get inserted api secret
        var apiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDto.Value && x.ApiResource.Id == newApiResourceDto.Id)
            .SingleOrDefaultAsync();

        //Map entity to model
        var secretsDto = apiSecret.ToModel();

        //Get new api secret    
        var newApiSecret = await apiResourceService.GetApiSecretAsync(secretsDto.ApiSecretId);

        //Assert secret value
        secretsDto.Value.Should().Be(apiSecretsDto.Value);

        //Assert
        secretsDto.Should().BeEquivalentTo(newApiSecret, o => o.Excluding(x => x.ApiResourceName).Excluding(x => x.Value));
    }

    [Fact]
    public async Task DeleteApiSecretAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResourceDto);

        //Get new api resource
        var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

        var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

        //Assert new api resource
        newApiResourceDto.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

        //Generate random new api secret
        var apiSecretsDtoMock = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

        //Add new api secret
        await apiResourceService.AddApiSecretAsync(apiSecretsDtoMock);

        //Get inserted api secret
        var apiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id)
            .SingleOrDefaultAsync();

        //Map entity to model
        var apiSecretsDto = apiSecret.ToModel();

        //Get new api secret    
        var newApiSecret = await apiResourceService.GetApiSecretAsync(apiSecretsDto.ApiSecretId);

        //Assert
        apiSecretsDto.Should().BeEquivalentTo(newApiSecret, o => o.Excluding(x => x.ApiResourceName).Excluding(x => x.Value));

        apiSecretsDto.Value.Should().Be(apiSecretsDtoMock.Value);

        //Delete it
        await apiResourceService.DeleteApiSecretAsync(newApiSecret);

        var deletedApiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id)
            .SingleOrDefaultAsync();

        //Assert after deleting
        deletedApiSecret.Should().BeNull();
    }

    [Fact]
    public async Task AddApiResourcePropertyAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResource = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResource);

        //Get new api resource
        var resource = await context.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();

        var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

        //Assert new api resource
        apiResourceDto.Should().BeEquivalentTo(apiResource, options => options.Excluding(o => o.Id));

        //Generate random new api resource property
        var apiResourceProperty = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

        //Add new api resource property
        await apiResourceService.AddApiResourcePropertyAsync(apiResourceProperty);

        //Get inserted api resource property
        var property = await context.ApiResourceProperties.Where(x => x.Value == apiResourceProperty.Value && x.ApiResource.Id == resource.Id)
            .SingleOrDefaultAsync();

        //Map entity to model
        var propertyDto = property.ToModel();

        //Get new api resource property    
        var resourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

        //Assert
        propertyDto.Should().BeEquivalentTo(resourcePropertiesDto, options =>
            options.Excluding(o => o.ApiResourcePropertyId)
                   .Excluding(o => o.ApiResourceName));
    }

    [Fact]
    public async Task GetApiResourcePropertyAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResource = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResource);

        //Get new api resource
        var resource = await context.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();

        var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

        //Assert new api resource
        apiResourceDto.Should().BeEquivalentTo(apiResource, options => options.Excluding(o => o.Id));

        //Generate random new api resource property
        var apiResourceProperty = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

        //Add new api resource property
        await apiResourceService.AddApiResourcePropertyAsync(apiResourceProperty);

        //Get inserted api resource property
        var property = await context.ApiResourceProperties.Where(x => x.Value == apiResourceProperty.Value && x.ApiResource.Id == resource.Id)
            .SingleOrDefaultAsync();

        //Map entity to model
        var propertyDto = property.ToModel();

        //Get new api resource property    
        var apiResourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

        //Assert
        propertyDto.Should().BeEquivalentTo(apiResourcePropertiesDto, options =>
            options.Excluding(o => o.ApiResourcePropertyId)
            .Excluding(o => o.ApiResourceName));
    }

    [Fact]
    public async Task DeleteApiResourcePropertyAsync()
    {
        using var context = GetDbContext();
        var apiResourceService = GetApiResourceService(context);

        //Generate random new api resource
        var apiResource = ApiResourceDtoMock.GenerateRandomApiResource(0);

        await apiResourceService.AddApiResourceAsync(apiResource);

        //Get new api resource
        var resource = await context.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();

        var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

        //Assert new api resource
        apiResourceDto.Should().BeEquivalentTo(apiResource, options => options.Excluding(o => o.Id));

        //Generate random new api resource Property
        var apiResourcePropertiesDto = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

        //Add new api resource Property
        await apiResourceService.AddApiResourcePropertyAsync(apiResourcePropertiesDto);

        //Get inserted api resource Property
        var property = await context.ApiResourceProperties.Where(x => x.Value == apiResourcePropertiesDto.Value && x.ApiResource.Id == resource.Id)
            .SingleOrDefaultAsync();

        //Map entity to model
        var propertiesDto = property.ToModel();

        //Get new api resource Property    
        var resourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

        //Assert
        propertiesDto.Should().BeEquivalentTo(resourcePropertiesDto, options =>
            options.Excluding(o => o.ApiResourcePropertyId)
            .Excluding(o => o.ApiResourceName));

        //Delete api resource Property
        await apiResourceService.DeleteApiResourcePropertyAsync(resourcePropertiesDto);

        //Get removed api resource Property
        var apiResourceProperty = await context.ApiResourceProperties.Where(x => x.Id == property.Id).SingleOrDefaultAsync();

        //Assert after delete it
        apiResourceProperty.Should().BeNull();
    }
}