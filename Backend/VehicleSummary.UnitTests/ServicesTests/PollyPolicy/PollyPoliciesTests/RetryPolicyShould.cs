using FluentAssertions;
using Polly.Retry;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VehicleSummary.Api.Services.PollyPolicy;
using Xunit;

namespace VehicleSummary.UnitTests.ServicesTests.PollyPolicy.PollyPoliciesTests
{
    public class RetryPolicyShould
    {
        private const string InvocationCounterHeaderName = "X-Invocation-Counter";

        [Theory]
        [InlineData(0, HttpStatusCode.RequestTimeout, "1", HttpStatusCode.OK)]
        [InlineData(1, HttpStatusCode.RequestTimeout, "2", HttpStatusCode.OK)]
        [InlineData(2, HttpStatusCode.RequestTimeout, "3", HttpStatusCode.OK)]
        [InlineData(3, HttpStatusCode.RequestTimeout, "4", HttpStatusCode.OK)]
        [InlineData(4, HttpStatusCode.RequestTimeout, "4", HttpStatusCode.RequestTimeout)]
        [InlineData(5, HttpStatusCode.RequestTimeout, "4", HttpStatusCode.RequestTimeout)]
        [InlineData(6, HttpStatusCode.RequestTimeout, "4", HttpStatusCode.RequestTimeout)]
        [InlineData(0, HttpStatusCode.InternalServerError, "1", HttpStatusCode.OK)]
        [InlineData(1, HttpStatusCode.InternalServerError, "2", HttpStatusCode.OK)]
        [InlineData(2, HttpStatusCode.InternalServerError, "3", HttpStatusCode.OK)]
        [InlineData(3, HttpStatusCode.InternalServerError, "4", HttpStatusCode.OK)]
        [InlineData(4, HttpStatusCode.InternalServerError, "4", HttpStatusCode.InternalServerError)]
        [InlineData(5, HttpStatusCode.InternalServerError, "4", HttpStatusCode.InternalServerError)]
        [InlineData(6, HttpStatusCode.InternalServerError, "4", HttpStatusCode.InternalServerError)]
        [InlineData(0, HttpStatusCode.BadGateway, "1", HttpStatusCode.OK)]
        [InlineData(1, HttpStatusCode.BadGateway, "2", HttpStatusCode.OK)]
        [InlineData(2, HttpStatusCode.BadGateway, "3", HttpStatusCode.OK)]
        [InlineData(3, HttpStatusCode.BadGateway, "4", HttpStatusCode.OK)]
        [InlineData(4, HttpStatusCode.BadGateway, "4", HttpStatusCode.BadGateway)]
        [InlineData(5, HttpStatusCode.BadGateway, "4", HttpStatusCode.BadGateway)]
        [InlineData(6, HttpStatusCode.BadGateway, "4", HttpStatusCode.BadGateway)]
        [InlineData(0, HttpStatusCode.ServiceUnavailable, "1", HttpStatusCode.OK)]
        [InlineData(1, HttpStatusCode.ServiceUnavailable, "2", HttpStatusCode.OK)]
        [InlineData(2, HttpStatusCode.ServiceUnavailable, "3", HttpStatusCode.OK)]
        [InlineData(3, HttpStatusCode.ServiceUnavailable, "4", HttpStatusCode.OK)]
        [InlineData(4, HttpStatusCode.ServiceUnavailable, "4", HttpStatusCode.ServiceUnavailable)]
        [InlineData(5, HttpStatusCode.ServiceUnavailable, "4", HttpStatusCode.ServiceUnavailable)]
        [InlineData(6, HttpStatusCode.ServiceUnavailable, "4", HttpStatusCode.ServiceUnavailable)]
        [InlineData(0, HttpStatusCode.GatewayTimeout, "1", HttpStatusCode.OK)]
        [InlineData(1, HttpStatusCode.GatewayTimeout, "2", HttpStatusCode.OK)]
        [InlineData(2, HttpStatusCode.GatewayTimeout, "3", HttpStatusCode.OK)]
        [InlineData(3, HttpStatusCode.GatewayTimeout, "4", HttpStatusCode.OK)]
        [InlineData(4, HttpStatusCode.GatewayTimeout, "4", HttpStatusCode.GatewayTimeout)]
        [InlineData(5, HttpStatusCode.GatewayTimeout, "4", HttpStatusCode.GatewayTimeout)]
        [InlineData(6, HttpStatusCode.GatewayTimeout, "4", HttpStatusCode.GatewayTimeout)]
        [Trait("Category", "Unit")]
        public async Task Retry_when_specified_error_occurs(
            int numberOfFailureTimes,
            HttpStatusCode errorStatusCodeToReturn,
            string expectedNumberOfInvocations,
            HttpStatusCode expectedStatusCode
            )
        {
            // Arrange
            var sut = GetSystemUnderTest();

            // Act
            var actual = await sut
                .ExecuteAsync(MockFlakyApiCall(errorStatusCodeToReturn, numberOfFailureTimes));

            // Assert
            actual.StatusCode.Should().Be(expectedStatusCode);
            GetInvocationCounterFromMessageHeader(actual).Should().Be(expectedNumberOfInvocations);
        }


        [Theory]
        [InlineData(0, HttpStatusCode.NotFound, "1", HttpStatusCode.OK)]
        [InlineData(1, HttpStatusCode.NotFound, "1", HttpStatusCode.NotFound)]
        [InlineData(2, HttpStatusCode.NotFound, "1", HttpStatusCode.NotFound)]
        [InlineData(3, HttpStatusCode.NotFound, "1", HttpStatusCode.NotFound)]
        [InlineData(4, HttpStatusCode.NotFound, "1", HttpStatusCode.NotFound)]
        [Trait("Category", "Unit")]
        public async Task Not_retry_when_unspecified_error_occurs(
            int numberOfFailureTimes,
            HttpStatusCode errorStatusCodeToMock,
            string expectedNumberOfInvocations,
            HttpStatusCode expectedStatusCode
            )
        {
            // Arrange
            var sut = GetSystemUnderTest();

            // Act
            var actual = await sut
                .ExecuteAsync(MockFlakyApiCall(errorStatusCodeToMock, numberOfFailureTimes));

            // Assert
            actual.StatusCode.Should().Be(expectedStatusCode);
            GetInvocationCounterFromMessageHeader(actual).Should().Be(expectedNumberOfInvocations);
        }

        private static string GetInvocationCounterFromMessageHeader(HttpResponseMessage responseMessage)
        {
            return responseMessage.Headers.GetValues(InvocationCounterHeaderName).FirstOrDefault();
        }

        private static AsyncRetryPolicy<HttpResponseMessage> GetSystemUnderTest()
        {
            PollyPolicies pollyPolicies = new()
            {
                // stub a custom algorithm to make the unit tests fast
                SleepDurationProvider = retryAttempt => TimeSpan.FromSeconds(1)
            };
            return pollyPolicies.RetryPolicy;
        }

        private static Func<Task<HttpResponseMessage>> MockFlakyApiCall(
            HttpStatusCode errorStatusCodeToMock,
            int numberOfFailureTimes)
        {
            int invocations = 0;
            return () =>
            {
                HttpResponseMessage responseMessage = new();
                if (invocations < numberOfFailureTimes)
                {
                    responseMessage.StatusCode = errorStatusCodeToMock;
                }
                else
                {
                    responseMessage.StatusCode = HttpStatusCode.OK;
                }
                invocations++;
                responseMessage.Headers.Add(InvocationCounterHeaderName, invocations.ToString());
                return Task.FromResult(responseMessage);
            };
        }
    }
}
