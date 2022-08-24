using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VehicleSummary.Api.Controllers;
using VehicleSummary.Api.Exceptions;
using VehicleSummary.UnitTests.Helpers;
using Xunit;

namespace VehicleSummary.UnitTests.ControllersTests.ErrorControllerTests
{
    public class HandleErrorShould
    {
        private readonly Fixture _autoFixture = new();

        private ErrorController CreateSystemUnderTest() => new();

        [Fact]
        [Trait("Category", "Unit")]
        public void Return_client_error_NotFound_when_WebApiCallException_occurs_with_NotFound_error()
        {
            //Arrange
            var arbitraryMessageInResponseBody = _autoFixture.Create<string>();

            ErrorController sut = CreateSystemUnderTest();
            IExceptionHandlerFeature exceptionHandlerFeature =
                new ExceptionHandlerFeature 
                { 
                    Error = new WebApiCallException()
                    {
                        ResponseStatusCode = HttpStatusCode.NotFound,
                        MessageInResponseBody = arbitraryMessageInResponseBody
                    }
                };
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            sut.ControllerContext.HttpContext.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);

            //Act
            var actual = sut.HandleError();

            //Assert
            actual.Should().BeAssignableTo<IActionResult>();
            var actualObjectResult = actual as ObjectResult;
            var actualInnerObjectResult = actualObjectResult.Value as ObjectResult;
            var actualProblemDetails = actualInnerObjectResult.Value as ProblemDetails;
            actualProblemDetails.Detail.Should().Contain(arbitraryMessageInResponseBody);
            actualProblemDetails.Title.Should().Contain("Client");
            actualProblemDetails.Status.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Return_server_error_when_WebApiCallException_occurs_with_server_error()
        {
            //Arrange
            var arbitraryMessage = _autoFixture.Create<string>();
            HttpStatusCode arbitraryErrorHttpStatus = RandomValuesGenerator.GetRandomErrorStatusCode(new List<int> { 404 });

            ErrorController sut = CreateSystemUnderTest();
            IExceptionHandlerFeature exceptionHandlerFeature =
                new ExceptionHandlerFeature
                {
                    Error = new WebApiCallException(arbitraryMessage)
                    {
                        ResponseStatusCode = arbitraryErrorHttpStatus
                    }
                };
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            sut.ControllerContext.HttpContext.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);

            //Act
            var actual = sut.HandleError();

            //Assert
            actual.Should().BeAssignableTo<IActionResult>();
            var actualObjectResult = actual as ObjectResult;
            var actualInnerObjectResult = actualObjectResult.Value as ObjectResult;
            var actualProblemDetails = actualInnerObjectResult.Value as ProblemDetails;
            actualProblemDetails.Detail.Should().Contain("Ooops");
            actualProblemDetails.Title.Should().Contain("Server");
            actualProblemDetails.Status.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        //TODO: test more edgy cases, boundary cases, like 502, 503, 500, and other Exceptions
    }
}
