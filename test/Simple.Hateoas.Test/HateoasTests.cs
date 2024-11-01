﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using Moq.AutoMock;
using Simple.Hateoas.Internal;
using Simple.Hateoas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Simple.Hateoas.Tests
{
    public class HateoasTests
    {
        readonly AutoMocker _mocker;

        public HateoasTests()
        {
            _mocker = new AutoMocker();

            _mocker.GetMock<IHttpContextAccessor>()
                .Setup(_ => _.HttpContext)
                .Returns(_mocker.GetMock<HttpContext>().Object);

            _mocker.GetMock<LinkGenerator>();

            _mocker.GetMock<IHateoasBuilderContext>()
                .Setup(_ => _.GetHateoasLinkBuilderInstance<CustomerMockDto>(It.IsAny<Type>()))
                .Returns(new CustomerMockDtoHateoasLinkBuilder());
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

        [Fact(DisplayName = "Should create hateoas result with args")]
        [Trait("Public", "Hateoas Tests")]
        public void Hateoas_CreateHateoasResult_ShouldCreateWithArgs()
        {
            // Arrange     
            var hateoas = _mocker.CreateInstance<Hateoas>();

            // Act
            var hateoasResult = hateoas.Create(new CustomerMockDto(2), "Arg01", "Arg02");

            // Assert
            Assert.NotNull(hateoasResult);
            Assert.Equal(2, hateoasResult.GetTotalArgs());
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

        public HateoasResult<CustomerMockDto> AddLinks(HateoasResult<CustomerMockDto> hateoasResult)
        {
            hateoasResult
                .AddSelfLink(RouteName1, _ => new { id = _.Id })
                .AddLink(RouteName2, HttpMethod.Patch)
                .AddLink(RouteName3, HttpMethod.Delete, _ => _.Id == 2);

            return hateoasResult;
        }
    }
    #endregion
}