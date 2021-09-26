using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Simple.Hateoas.Internal;
using Simple.Hateoas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Simple.Hateoas.Tests.IServiceRouteMock;

namespace Simple.Hateoas.Tests
{
    public class HateoasTests
    {
        readonly AutoMocker _mocker;

        public HateoasTests()
        {
            _mocker = new AutoMocker();

            _mocker.GetMock<IUrlHelper>()
                .Setup(_ => _.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("https://mock-link.test");

            _mocker.GetMock<IHateoasBuilderContext>()
                .Setup(_ => _.GetHateoasLinkBuilderType(It.IsAny<Type>()))
                .Returns(typeof(CustomerMockDtoHateoasLinkBuilder));
        }

        [Fact(DisplayName = "Should create all links of IHateoasLinkBuilder")]
        [Trait("Public", "Hateoas Tests")]
        public void Hateoas_CreateHateoasResult_ShouldCreateAllLinks()
        {
            // Arrange     
            var hateoas = _mocker.CreateInstance<Hateoas>();

            // Act
            var hateoasResult = hateoas.Create(new CustomerMockDto(2));

            // Assert
            Assert.NotNull(hateoasResult);
            Assert.NotEmpty(hateoasResult.Links);
            Assert.Equal(3, hateoasResult.Links.Count);
        }

        [Fact(DisplayName = "Should create links without when predicate of IHateoasLinkBuilder")]
        [Trait("Public", "Hateoas Tests")]
        public void Hateoas_CreateHateoasResult_ShouldCreateLinksWithoutPredicate()
        {
            // Arrange
            var hateoas = _mocker.CreateInstance<Hateoas>();

            // Act
            var hateoasResult = hateoas.Create(new CustomerMockDto(1));

            // Assert
            Assert.NotNull(hateoasResult);
            Assert.NotEmpty(hateoasResult.Links);
            Assert.Equal(2, hateoasResult.Links.Count);
            Assert.DoesNotContain(CustomerMockDtoHateoasLinkBuilder.RouteName3, hateoasResult.Links.Select(_ => _.Rel));
        }

        [Fact(DisplayName = "Try create links without IHateoasLinkBuilder")]
        [Trait("Public", "Hateoas Tests")]
        public void Hateoas_CreateHateoasResult_ShouldNotCreateLinks()
        {
            // Arrange
            var hateoas = _mocker.CreateInstance<Hateoas>();
            var error = false;

            // Act
            try
            {
                hateoas.Create(new List<CustomerMockDto> { new CustomerMockDto(0) });

            }
            catch (Exception)
            {
                error = true;
            }

            // Assert
            Assert.True(error);
        }

        [Fact(DisplayName = "Should create links of IHateoasLinkBuilder with dependecy injection")]
        [Trait("Public", "Hateoas Tests")]
        public void Hateoas_CreateHateoasResult_ShouldCreateLinksWithDI()
        {
            // Arrange
            _mocker.GetMock<IHateoasBuilderContext>()
                .Setup(_ => _.GetHateoasLinkBuilderType(It.IsAny<Type>()))
                .Returns(typeof(DependencyInjectionHateoasLinkBuilder));

            _mocker.GetMock<IServiceProvider>()
                .Setup(_ => _.GetService(typeof(IServiceRouteMock)))
                .Returns(new ServiceRouteMock());

            var hateoas = _mocker.CreateInstance<Hateoas>();

            // Act
            var hateoasResult = hateoas.Create(new CustomerMockDto(2));

            // Assert
            Assert.NotNull(hateoasResult);
            Assert.NotEmpty(hateoasResult.Links);
            Assert.Contains("anything_route_name", hateoasResult.Links.Select(_ => _.Rel));
        }
    }

    #region Mock Data
    public class CustomerMockDto
    {
        public CustomerMockDto(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    public class CustomerMockDtoHateoasLinkBuilder : IHateoasLinkBuilder<CustomerMockDto>
    {
        public const string RouteName1 = "mock-route-name-1";
        public const string RouteName2 = "mock-route-name-2";
        public const string RouteName3 = "mock-route-name-3";

        public HateoasResult<CustomerMockDto> Build(HateoasResult<CustomerMockDto> hateoasResult)
        {
            hateoasResult
                .AddSelfLink(RouteName1, _ => new { id = _.Id })
                .AddLink(RouteName2, HttpMethod.Patch)
                .AddLink(RouteName3, HttpMethod.Delete, _ => _.Id == 2);

            return hateoasResult;
        }
    }


    public interface IServiceRouteMock
    {
        string GetRouteName();

        public class ServiceRouteMock : IServiceRouteMock
        {
            public string GetRouteName() => "anything_route_name";
        }
    }

    public class DependencyInjectionHateoasLinkBuilder : IHateoasLinkBuilder<CustomerMockDto>
    {
        private readonly IServiceRouteMock _serviceMock;

        public DependencyInjectionHateoasLinkBuilder(IServiceRouteMock serviceMock)
        {
            _serviceMock = serviceMock;
        }

        public HateoasResult<CustomerMockDto> Build(HateoasResult<CustomerMockDto> hateoasResult)
        {
            hateoasResult.AddLink(_serviceMock.GetRouteName(), HttpMethod.Delete);
            return hateoasResult;
        }
    }
    #endregion
}