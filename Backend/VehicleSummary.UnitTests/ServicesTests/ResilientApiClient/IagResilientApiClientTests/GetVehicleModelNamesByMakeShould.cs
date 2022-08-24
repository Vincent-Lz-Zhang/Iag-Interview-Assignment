using AutoFixture;
using FluentAssertions;
using Flurl.Http.Testing;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Exceptions;
using VehicleSummary.Api.Models;
using VehicleSummary.Api.Services.ResilientApiClient;
using VehicleSummary.UnitTests.Helpers;
using Xunit;

namespace VehicleSummary.UnitTests.ServicesTests.ResilientApiClient.IagResilientApiClientTests
{
    public class GetVehicleModelNamesByMakeShould
    {
        private const string ExpectedKeywordContainedInMessage = "IAG API";
        private readonly Fixture _autoFixture = new();
        private readonly Mock<IOptions<VehicleApiOptions>> _optionsStub;
        private readonly VehicleApiOptions _options;

        public GetVehicleModelNamesByMakeShould()
        {
            _optionsStub = new Mock<IOptions<VehicleApiOptions>>();
            _options = new()
            {
                GetModelsByMakeUrl = "http://hostname/apipath/makes/{0}/models",
                GetModelsByMakeApiVersion = _autoFixture.Create<string>(),
                ApiVersionParamName = _autoFixture.Create<string>(),
                ApiKeyHeaderName = _autoFixture.Create<string>(),
                ApiKey = _autoFixture.Create<string>()
            };
            _optionsStub.Setup(o => o.Value).Returns(_options);
        }

        private IagResilientApiClient CreateSystemUnderTest()
        {
            return new IagResilientApiClient(_optionsStub.Object);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Return_the_list_of_model_name_when_api_returns_a_list_of_model_name()
        {
            using var httpTest = new HttpTest();

            // Arrange
            var arbitraryMake = _autoFixture.Create<string>();
            var arbitraryModelName1 = _autoFixture.Create<string>();
            var arbitraryModelName2 = _autoFixture.Create<string>();
            var modelNameList = new string[] { arbitraryModelName1, arbitraryModelName2 };

            IagResilientApiClient sut = CreateSystemUnderTest();

            httpTest.RespondWithJson(modelNameList);

            // Act
            var actual = await sut.GetVehicleModelNamesByMake(arbitraryMake, CancellationToken.None);

            // Assert
            httpTest
                .ShouldHaveCalled($"http://hostname/apipath/makes/{arbitraryMake}/models?" +
                    $"{_options.ApiVersionParamName}={_options.GetModelsByMakeApiVersion}")
                .WithHeader(_options.ApiKeyHeaderName, _options.ApiKey);

            actual.Should().BeEquivalentTo(new[] { arbitraryModelName1, arbitraryModelName2 });
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Throw_WebApiCallException_when_Flurl_throws_TimeOutException()
        {
            using var httpTest = new HttpTest();

            // Arrange
            var arbitraryMake = _autoFixture.Create<string>();

            var sut = CreateSystemUnderTest();

            httpTest.SimulateTimeout();

            // Act
            var actualException = await Assert.ThrowsAsync<WebApiCallException>(
                async () => await sut.GetVehicleModelNamesByMake(
                    arbitraryMake,
                    CancellationToken.None)
            );

            // Assert
            httpTest
                .ShouldHaveCalled($"http://hostname/apipath/makes/{arbitraryMake}/models?" +
                    $"{_options.ApiVersionParamName}={_options.GetModelsByMakeApiVersion}")
                .WithHeader(_options.ApiKeyHeaderName, _options.ApiKey);

            var actualWebApiException = actualException.As<WebApiCallException>();
            actualWebApiException.Message.Should().Contain(ExpectedKeywordContainedInMessage);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Throw_WebApiCallException_when_Flurl_throws_FlurlHttpException()
        {
            using var httpTest = new HttpTest();

            // Arrange
            var arbitraryMake = _autoFixture.Create<string>();
            var arbitraryMessage = _autoFixture.Create<string>();
            var arbitraryIsTransient = _autoFixture.Create<bool>();
            var vehicleApiErrorResponse = new VehicleApiErrorResponse()
            {
                Message = arbitraryMessage,
                IsTransient = arbitraryIsTransient
            };
            var arbitraryCorrelationId = _autoFixture.Create<string>();
            HttpStatusCode arbitraryErrorHttpStatus = RandomValuesGenerator.GetRandomErrorStatusCode();

            var sut = CreateSystemUnderTest();

            httpTest
                .RespondWithJson(
                    vehicleApiErrorResponse, 
                    (int)arbitraryErrorHttpStatus,
                    new List<(string key, string value)>() 
                    { 
                        (
                            ApiConstants.CorrelationId, 
                            arbitraryCorrelationId
                        ) 
                    }
                );

            // Act
            var actualException = await Assert.ThrowsAsync<WebApiCallException>(
                async () => await sut.GetVehicleModelNamesByMake(
                    arbitraryMake, 
                    CancellationToken.None)
            );

            // Assert
            httpTest
                .ShouldHaveCalled($"http://hostname/apipath/makes/{arbitraryMake}/models?" +
                    $"{_options.ApiVersionParamName}={_options.GetModelsByMakeApiVersion}")
                .WithHeader(_options.ApiKeyHeaderName, _options.ApiKey);

            var actualWebApiException = actualException.As<WebApiCallException>();
            actualWebApiException.ResponseStatusCode.Should().Be(arbitraryErrorHttpStatus);
            actualWebApiException.MessageInResponseBody.Should().Be(arbitraryMessage);
            actualWebApiException.CorrelationId.Should().Be(arbitraryCorrelationId);
            actualWebApiException.Message.Should().Contain(ExpectedKeywordContainedInMessage);
        }

        //TODO: test more edgy cases, boundary cases, like 502, 503, 500, and other Exceptions
        
    }
}
