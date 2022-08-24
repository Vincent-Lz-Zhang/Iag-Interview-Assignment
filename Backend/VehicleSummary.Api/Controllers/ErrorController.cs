using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using VehicleSummary.Api.Exceptions;
using VehicleSummary.Api.Models;

namespace VehicleSummary.Api.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
            
            return GenerateErrorResponseAccordingToError(exceptionHandlerFeature?.Error);
        }

        /// <remarks>
        /// Centralize error handling, and logging.
        /// </remarks>
        private ObjectResult GenerateErrorResponseAccordingToError(Exception? ex)
        {
            if (ex is null)
            {
                return CreateServerSideErrorResult();
            }

            if (ex is WebApiCallException webApiCallException)
            {
                if (webApiCallException.ResponseStatusCode == HttpStatusCode.NotFound)
                {
                    ForwardCorrelationIdInResponse(webApiCallException);
                    // TODO: log the error with Correlation Id

                    return new NotFoundObjectResult(Problem(
                           detail: $"IAG API responded: {webApiCallException.MessageInResponseBody}",
                           title: "Client mistake",
                           statusCode: (int?)webApiCallException.ResponseStatusCode))
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                }

                // other statuses like 400, 409, 422
            }
            // other errors that we want to treat as client mistake, 

            // in case of server side errors
            // TODO: log the server error 

            return CreateServerSideErrorResult();
        }

        private ObjectResult CreateServerSideErrorResult()
        {
            return new ObjectResult(Problem(
                                detail: "Ooops!",
                                title: "Server error",
                                statusCode: (int)HttpStatusCode.InternalServerError));
        }

        private void ForwardCorrelationIdInResponse(WebApiCallException webApiCallException)
        {
            if (!string.IsNullOrEmpty(webApiCallException.CorrelationId))
            {
                Response.Headers.Add(ApiConstants.CorrelationId, webApiCallException.CorrelationId);
            }
        }
    }
}
