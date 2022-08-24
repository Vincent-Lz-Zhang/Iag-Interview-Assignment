using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VehicleSummary.Api;
using VehicleSummary.Api.Models;
using Xunit;

namespace VehicleSummary.IntegrationTests.VehicleChecksControllerTests
{
    public class VehicleChecksMakesShould : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private const string Uri = "http://localhost/api/v1/vehicle-checks/makes/";

        public VehicleChecksMakesShould(WebApplicationFactory<Program> factory)
        {
            factory.ClientOptions.BaseAddress = new Uri(Uri);
            _factory = factory;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Return_expected_response_when_dependency_api_works()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // TODO: mock the IAG API and inject the mock here
                    // Ideally, we should use a mock to simulate the IAG API for this testing
                });
            }).CreateClient();

            var validMake = "Lotus";
            var validModelOfTheMake = "Elise";

            // Act
            var actual = await client.GetFromJsonAsync<VehicleSummaryResponse>(validMake);

            // Assert
            actual.Make.Should().Be(validMake);
            actual.Models.Should().HaveCountGreaterThan(1);
            actual.Models.Should().Contain(m => m.Name == validModelOfTheMake && m.YearsAvailable > 1900);
        }

        // TODO: test other cases, like transient Http errors (404), and check header for correlation id
    }
}