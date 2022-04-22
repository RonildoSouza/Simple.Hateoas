using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Simple.Hateoas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Simple.Hateoas.Internal.Tests
{
    public class HateoasBuilderContextTests
    {
        readonly AutoMocker _mocker;

        public HateoasBuilderContextTests()
        {
            _mocker = new AutoMocker();

            _mocker.GetMock<IUrlHelper>()
                .Setup(_ => _.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("https://mock-link.test");
        }

        [Fact(DisplayName = "Success initialized HateoasBuilderContext")]
        [Trait("Internal", "HateoasBuilderContext Tests")]
        public void HateoasBuilderContext_NewInstance_ShouldSucessInitialize()
        {
            // Arrange
            var serviceProviderMock = _mocker.GetMock<IServiceProvider>();
            var hateoasBuilderContext = new HateoasBuilderContext(typeof(HateoasBuilderContextTests).Assembly);
            hateoasBuilderContext.SetServiceProvider(serviceProviderMock.Object);

            // Act
            var customerMockDtoHateoasLinkBuilder = hateoasBuilderContext.GetHateoasLinkBuilderInstance<CustomerMockDto>(typeof(IHateoasLinkBuilder<CustomerMockDto>));
            var listCustomerMockDtoHateoasLinkBuilder = hateoasBuilderContext.GetHateoasLinkBuilderInstance<IEnumerable<CustomerMockDto>>(typeof(IHateoasLinkBuilder<IEnumerable<CustomerMockDto>>));

            // Assert
            Assert.NotNull(customerMockDtoHateoasLinkBuilder);
            Assert.NotNull(listCustomerMockDtoHateoasLinkBuilder);
            Assert.True(customerMockDtoHateoasLinkBuilder.GetType() == typeof(CustomerMockDtoHateoasLinkBuilder), $"Invalid typeof {nameof(CustomerMockDtoHateoasLinkBuilder)}");
            Assert.True(listCustomerMockDtoHateoasLinkBuilder.GetType() == typeof(ListCustomerMockDtoHateoasLinkBuilder), $"Invalid typeof {nameof(ListCustomerMockDtoHateoasLinkBuilder)}");
        }


        [Fact(DisplayName = "Should create links of IHateoasLinkBuilder with dependecy injection")]
        [Trait("Internal", "HateoasBuilderContext Tests")]
        public void Hateoas_CreateHateoasResult_ShouldCreateLinksWithDI()
        {
            // Arrange
            var serviceRouteMock = new IServiceRouteMock.ServiceRouteMock();
            var serviceProviderMock = _mocker.GetMock<IServiceProvider>();
            serviceProviderMock.Setup(_ => _.GetService(typeof(IServiceRouteMock))).Returns(serviceRouteMock);

            var hateoasBuilderContext = new HateoasBuilderContext(typeof(HateoasBuilderContextTests).Assembly);
            hateoasBuilderContext.SetServiceProvider(serviceProviderMock.Object);

            // Act
            var dependencyInjectionHateoasLinkBuilder = hateoasBuilderContext.GetHateoasLinkBuilderInstance<object>(typeof(IHateoasLinkBuilder<object>));
            var hateoasResult = dependencyInjectionHateoasLinkBuilder.AddLinks(new HateoasResult<object>(_mocker.Get<IUrlHelper>(), null));

            // Assert
            Assert.NotNull(hateoasResult);
            Assert.Contains("anything_route_name", hateoasResult.Links.Select(_ => _.Rel));
            serviceProviderMock.Verify(_ => _.GetService(typeof(IServiceRouteMock)), Times.Once);
        }
    }

    #region Mock Data
    public class CustomerMockDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomerMockDtoHateoasLinkBuilder : IHateoasLinkBuilder<CustomerMockDto>
    {
        public HateoasResult<CustomerMockDto> AddLinks(HateoasResult<CustomerMockDto> hateoasResult) => hateoasResult;
    }

    public class ListCustomerMockDtoHateoasLinkBuilder : IHateoasLinkBuilder<IEnumerable<CustomerMockDto>>
    {
        public HateoasResult<IEnumerable<CustomerMockDto>> AddLinks(HateoasResult<IEnumerable<CustomerMockDto>> hateoasResult) => hateoasResult;
    }


    public interface IServiceRouteMock
    {
        string GetRouteName();

        public class ServiceRouteMock : IServiceRouteMock
        {
            public string GetRouteName() => "anything_route_name";
        }
    }

    public class DependencyInjectionHateoasLinkBuilder : IHateoasLinkBuilder<object>
    {
        private readonly IServiceRouteMock _serviceMock;

        public DependencyInjectionHateoasLinkBuilder(IServiceRouteMock serviceMock)
        {
            _serviceMock = serviceMock;
        }

        public HateoasResult<object> AddLinks(HateoasResult<object> hateoasResult)
        {
            hateoasResult.AddLink(_serviceMock.GetRouteName(), HttpMethod.Delete);
            return hateoasResult;
        }
    }
    #endregion
}