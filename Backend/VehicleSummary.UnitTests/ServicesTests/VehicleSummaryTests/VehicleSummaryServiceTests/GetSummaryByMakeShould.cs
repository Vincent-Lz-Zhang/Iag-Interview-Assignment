using AutoFixture;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Exceptions;
using VehicleSummary.Api.Models;
using VehicleSummary.Api.Services.Interfaces;
using VehicleSummary.Api.Services.VehicleSummary;
using VehicleSummary.UnitTests.Helpers;
using Xunit;

namespace VehicleSummary.UnitTests.ServicesTests.VehicleSummaryTests.VehicleSummaryServiceTests
{
    public class GetSummaryByMakeShould
    {
        private readonly Fixture _autoFixture = new();
        private readonly Mock<IIagResilientApiClient> _iagResilientApiClientMock;

        public GetSummaryByMakeShould()
        {
            _iagResilientApiClientMock = new Mock<IIagResilientApiClient>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Return_the_list_of_VehicleSummaryResponse_when_Iag_api_works()
        {
            // Arrange
            var arbitraryMake = _autoFixture.Create<string>();

            var arbitraryModelName1 = _autoFixture.Create<string>();
            var arbitraryModelName2 = _autoFixture.Create<string>();

            var modelNameList = new List<string>() { arbitraryModelName1, arbitraryModelName2 };

            var arbitraryInt1 = _autoFixture.Create<int>();
            var arbitraryInt2 = _autoFixture.Create<int>();
            var yearsAvailableListOfModel1 = new List<int>() { arbitraryInt1, arbitraryInt2 };

            var arbitraryInt3 = _autoFixture.Create<int>();
            var arbitraryInt4 = _autoFixture.Create<int>();
            var yearsAvailableListOfModel2 = new List<int>() { arbitraryInt3, arbitraryInt4 };

            _iagResilientApiClientMock
                .Setup(
                    x => x.GetVehicleModelNamesByMake(
                        It.IsAny<string>(), 
                        It.IsAny<CancellationToken>()
                        )
                    )
                .ReturnsAsync(modelNameList);

            _iagResilientApiClientMock
                .Setup(
                    x => x.GetYearsAvailableByVehicleModel(
                        It.IsAny<string>(), 
                        It.Is<string>(m => m == arbitraryModelName1), 
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(yearsAvailableListOfModel1);

            _iagResilientApiClientMock
                .Setup(
                    x => x.GetYearsAvailableByVehicleModel(
                        It.IsAny<string>(), 
                        It.Is<string>(m => m == arbitraryModelName2), 
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(yearsAvailableListOfModel2);

            VehicleSummaryService sut = CreateSystemUnderTest();

            // Act
            var actual = await sut.GetSummaryByMake(arbitraryMake, CancellationToken.None);

            // Assert
            actual.Should().BeEquivalentTo(new VehicleSummaryResponse
                {
                    Make = arbitraryMake,
                    Models = new List<YearsAvailableOfVehicleModel>
                    {
                        new YearsAvailableOfVehicleModel { Name = arbitraryModelName1, YearsAvailable = arbitraryInt1},
                        new YearsAvailableOfVehicleModel { Name = arbitraryModelName1, YearsAvailable = arbitraryInt2},
                        new YearsAvailableOfVehicleModel { Name = arbitraryModelName2, YearsAvailable = arbitraryInt3},
                        new YearsAvailableOfVehicleModel { Name = arbitraryModelName2, YearsAvailable = arbitraryInt4}
                    }
                }
            );
            _iagResilientApiClientMock.Verify(c => c.GetVehicleModelNamesByMake(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _iagResilientApiClientMock.Verify(c => c.GetYearsAvailableByVehicleModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(modelNameList.Count));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Rethrow_WebApiCallException_when_Iag_api_client_throws_WebApiCallException()
        {
            // Arrange
            var arbitraryMake = _autoFixture.Create<string>();
            var arbitraryMessage = _autoFixture.Create<string>();
            var arbitraryMessageInResponseBody = _autoFixture.Create<string>();
            var arbitraryCorrelationId = _autoFixture.Create<string>();

            HttpStatusCode arbitraryErrorHttpStatus = RandomValuesGenerator.GetRandomErrorStatusCode();

            WebApiCallException webApiCallException = new(arbitraryMessage)
            {
                MessageInResponseBody = arbitraryMessageInResponseBody,
                ResponseStatusCode = arbitraryErrorHttpStatus,
                CorrelationId = arbitraryCorrelationId
            };

            _iagResilientApiClientMock
                .Setup(
                    x => x.GetVehicleModelNamesByMake(
                        It.IsAny<string>(), 
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync(webApiCallException);

            _iagResilientApiClientMock
                .Setup(
                    x => x.GetYearsAvailableByVehicleModel(
                        It.IsAny<string>(), 
                        It.IsAny<string>(), 
                        It.IsAny<CancellationToken>()
                    )
                )
                .Verifiable();

            VehicleSummaryService sut = CreateSystemUnderTest();

            // Act
            var actualException = await Assert.ThrowsAsync<WebApiCallException>(
                async () => await sut.GetSummaryByMake(arbitraryMake, CancellationToken.None)
            );

            // Assert
            var actualWebApiException = actualException.As<WebApiCallException>();
            actualWebApiException.MessageInResponseBody.Should().Be(arbitraryMessageInResponseBody);
            actualWebApiException.Message.Should().ContainAll(arbitraryMessage);
            actualWebApiException.CorrelationId.Should().ContainAll(arbitraryCorrelationId);
            actualWebApiException.ResponseStatusCode.Should().Be(arbitraryErrorHttpStatus);

            _iagResilientApiClientMock.Verify(
                x => x.GetVehicleModelNamesByMake(
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            _iagResilientApiClientMock.Verify(
                x => x.GetYearsAvailableByVehicleModel(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        // TODO: test more edgy cases, boundary cases, like and other Exceptions, and GetYearsAvailableByVehicleModel throwing exception

        private VehicleSummaryService CreateSystemUnderTest()
        {
            return new VehicleSummaryService(_iagResilientApiClientMock.Object);
        }
    }
}