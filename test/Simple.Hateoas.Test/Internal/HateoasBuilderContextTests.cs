using Simple.Hateoas.Models;
using System.Collections.Generic;
using Xunit;

namespace Simple.Hateoas.Internal.Tests
{
    public class HateoasBuilderContextTests
    {
        [Fact(DisplayName = "Success initialized HateoasBuilderContext")]
        [Trait("Internal", "HateoasBuilderContext Tests")]
        public void HateoasBuilderContext_NewInstance_ShouldSucessInitialize()
        {
            // Arrange
            var hateoasBuilderContext = new HateoasBuilderContext(typeof(HateoasBuilderContextTests).Assembly);

            // Act
            var customerMockDtoHateoasLinkBuilder = hateoasBuilderContext.GetHateoasLinkBuilderType(typeof(IHateoasLinkBuilder<CustomerMockDto>));
            var listCustomerMockDtoHateoasLinkBuilder = hateoasBuilderContext.GetHateoasLinkBuilderType(typeof(IHateoasLinkBuilder<IEnumerable<CustomerMockDto>>));

            // Assert
            Assert.NotNull(customerMockDtoHateoasLinkBuilder);
            Assert.NotNull(listCustomerMockDtoHateoasLinkBuilder);
            Assert.True(customerMockDtoHateoasLinkBuilder == typeof(CustomerMockDtoHateoasLinkBuilder), $"Invalid typeof {nameof(CustomerMockDtoHateoasLinkBuilder)}");
            Assert.True(listCustomerMockDtoHateoasLinkBuilder == typeof(ListCustomerMockDtoHateoasLinkBuilder), $"Invalid typeof {nameof(ListCustomerMockDtoHateoasLinkBuilder)}");
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
        public HateoasResult<CustomerMockDto> Build(HateoasResult<CustomerMockDto> hateoasResult) => hateoasResult;
    }

    public class ListCustomerMockDtoHateoasLinkBuilder : IHateoasLinkBuilder<IEnumerable<CustomerMockDto>>
    {
        public HateoasResult<IEnumerable<CustomerMockDto>> Build(HateoasResult<IEnumerable<CustomerMockDto>> hateoasResult) => hateoasResult;
    }
    #endregion
}