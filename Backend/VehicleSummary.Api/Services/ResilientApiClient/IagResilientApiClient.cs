using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Exceptions;
using VehicleSummary.Api.Models;
using VehicleSummary.Api.Services.Interfaces;

namespace VehicleSummary.Api.Services.ResilientApiClient
{
    /// <summary>
    /// It performs HTTP call in a resilient manner.
    /// </summary>

    /// <remarks>
    /// It is not thread-safe. The same instance can only be used in non-concurrent way.
    /// If you want to use it in concurrent way, 
    /// consider having a factory injected and using it to get multiple instances.
    /// </remarks>
    public class IagResilientApiClient : IIagResilientApiClient
    {
        private readonly VehicleApiOptions _options;

        public IagResilientApiClient(IOptions<VehicleApiOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<List<string>> GetVehicleModelNamesByMake(string make, CancellationToken cancellationToken)
        {
            var url = BuildFlurlUrl(_options.GetModelsByMakeApiVersion,
                _options.GetModelsByMakeUrl,
                make);

            FlurlRequest flurlRequest = BuildFlurlRequest(url);

            try
            {
                return await flurlRequest.GetJsonAsync<List<string>>(cancellationToken);
            }
            catch (FlurlHttpTimeoutException timeoutEx)
            {
                throw MapTimeoutExceptionToApiException(timeoutEx);
            }
            catch (FlurlHttpException flurlHttpEx)
            {
                throw await MapFlurlHttpExceptionToApiExceptionAsync(flurlHttpEx);
            }
        }

        public async Task<List<int>> GetYearsAvailableByVehicleModel(string make, string model, CancellationToken cancellationToken)
        {
            var url = BuildFlurlUrl(_options.GetYearsOfModelsByMakeApiVersion, 
                _options.GetYearsOfModelsByMakeUrl, 
                make, 
                model);
            
            FlurlRequest flurlRequest = BuildFlurlRequest(url);
            
            try
            {
                return await flurlRequest.GetJsonAsync<List<int>>(cancellationToken);
            }
            catch (FlurlHttpTimeoutException timeoutEx)
            {
                throw MapTimeoutExceptionToApiException(timeoutEx);
            }
            catch (FlurlHttpException flurlHttpEx)
            {
                /// <remarks>
                /// Use a custom exception to propagate the error information, and handle it in the error controller.
                /// To avoid exposing Flurl specific code throughout the codebase, separating concerns, isolating Flurl usage
                /// easy testing
                /// </remarks>
                throw await MapFlurlHttpExceptionToApiExceptionAsync(flurlHttpEx);
            }
        }

        private static WebApiCallException MapTimeoutExceptionToApiException(FlurlHttpTimeoutException timeoutEx)
        {
            // TODO: log the exception

            WebApiCallException apiEx = new($"IAG API call timed out.");
            apiEx.MessageInResponseBody = timeoutEx.Message;

            return apiEx;
        }

        private static async Task<WebApiCallException> MapFlurlHttpExceptionToApiExceptionAsync(FlurlHttpException flurlHttpEx)
        {
            // TODO: log the exception

            WebApiCallException apiEx = new($"IAG API returns non-2xx response.");
            apiEx.ResponseStatusCode = (HttpStatusCode?)flurlHttpEx.StatusCode;

            /// <remarks>
            /// Assume that the error response format is uniform for IAG APIs.
            /// </remarks>
            var errorResponse = await flurlHttpEx.GetResponseJsonAsync<VehicleApiErrorResponse>();
            apiEx.MessageInResponseBody = errorResponse.Message;

            var correlationId = flurlHttpEx.Call?.Response?.Headers?.FirstOrDefault(ApiConstants.CorrelationId);
            if (!string.IsNullOrEmpty(correlationId))
            {
                apiEx.CorrelationId = correlationId;
            }
                
            return apiEx;
        }

        private Url BuildFlurlUrl(string apiVersion, string format, params object?[] args)
        {
            var url = string.Format(format, args)
                .SetQueryParam(_options.ApiVersionParamName, apiVersion, false);
            return url;
        }

        private FlurlRequest BuildFlurlRequest(Url url)
        {
            var flurlRequest = new FlurlRequest(url);
            flurlRequest.WithHeader(_options.ApiKeyHeaderName, _options.ApiKey);
            return flurlRequest;
        }

        // TODO: other authentication methods 
        // public FlurlWrapper WithOAuthBearerToken(string token)

    }
}
