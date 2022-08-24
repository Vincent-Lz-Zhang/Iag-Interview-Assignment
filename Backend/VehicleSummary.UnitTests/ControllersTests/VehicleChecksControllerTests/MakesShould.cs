using AutoFixture;
//using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Controllers;
using VehicleSummary.Api.Models;
using VehicleSummary.Api.ServicesInterfaces;
using Xunit;

namespace VehicleSummary.UnitTests.ControllersTests.VehicleChecksControllerTests
{
    public class MakesShould
    {
        private readonly Mock<IVehicleSummaryService> _vehicleSummaryServiceMock;
        private readonly Fixture _autoFixture = new();

        private VehicleChecksController CreateSystemUnderTest() => new(_vehicleSummaryServiceMock.Object);

        public MakesShould()
        {
            _vehicleSummaryServiceMock = new Mock<IVehicleSummaryService>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Call_VehicleSummaryService_with_given_make_and_return_its_response()
        {
            // Arrange
            var arbitaryMake = _autoFixture.Create<string>();
            var arbitaryModelName = _autoFixture.Create<string>();
            var arbitaryInteger = _autoFixture.Create<int>();

            VehicleChecksController sut = CreateSystemUnderTest();

            var responseStub = new VehicleSummaryResponse
            {
                Make = arbitaryMake,
                Models = new List<YearsAvailableOfVehicleModel>
                {
                    new YearsAvailableOfVehicleModel
                    {
                        Name = arbitaryModelName,
                        YearsAvailable = arbitaryInteger
                    }
                }
            };

            _vehicleSummaryServiceMock
                .Setup(
                    s => s.GetSummaryByMake(
                        It.IsAny<string>(), 
                        It.IsAny<CancellationToken>()
                    ))
                .ReturnsAsync(responseStub);

            // Act 
            var actual = await sut.Makes(arbitaryMake, CancellationToken.None);

            // Assert
            var actualActionResult = Assert.IsType<ActionResult<VehicleSummaryResponse>>(actual);
            var actualOkObjectResult = Assert.IsType<OkObjectResult>(actualActionResult.Result);
            var actualVehicleSummary = Assert.IsAssignableFrom<VehicleSummaryResponse>(actualOkObjectResult.Value);

            actualVehicleSummary.Should().BeEquivalentTo(
                new VehicleSummaryResponse
                {
                    Make = arbitaryMake,
                    Models = new List<YearsAvailableOfVehicleModel>
                    {
                        new YearsAvailableOfVehicleModel
                        {
                            Name = arbitaryModelName,
                            YearsAvailable = arbitaryInteger
                        }
                    }
                }
            );

            _vehicleSummaryServiceMock.Verify(
                x => x.GetSummaryByMake(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ), 
                Times.Once);
        }
    }
}