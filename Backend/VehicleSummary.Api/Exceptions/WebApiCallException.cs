using System;
using System.Net;

namespace VehicleSummary.Api.Exceptions
{
    public class WebApiCallException : Exception
    {
        public string? MessageInResponseBody { get; set; }
        public HttpStatusCode? ResponseStatusCode { get; set; }
        public string? CorrelationId { get; set; }

        public WebApiCallException()
        {
        }

        public WebApiCallException(string message)
            : base(message)
        {
        }

        public WebApiCallException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
