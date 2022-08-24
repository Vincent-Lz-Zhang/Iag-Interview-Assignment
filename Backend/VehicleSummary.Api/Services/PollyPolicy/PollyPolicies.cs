using Polly;
using Polly.Retry;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using VehicleSummary.Api.Services.Interfaces;

namespace VehicleSummary.Api.Services.PollyPolicy
{
    public class PollyPolicies : IPollyPolicies
    {
        public AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                return Policy
                    .HandleResult<HttpResponseMessage>(r => IsWorthRetrying(r))
                    .WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: _sleepDurationProvider,
                        onRetry: (response, time) =>
                        {
                            // TODO: log the error
                            Debug.WriteLine($"[App|Policy]: Retry delegate fired after {time.TotalSeconds:n1}s ({response?.Result?.StatusCode}).");
                        });
            }
        }

        public Func<int, TimeSpan> SleepDurationProvider 
        { 
            get => _sleepDurationProvider; 
            set => _sleepDurationProvider = value; 
        }

        private Func<int, TimeSpan> _sleepDurationProvider = retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));

        private bool IsWorthRetrying(HttpResponseMessage message)
        {
            switch (message.StatusCode)
            {
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    return true;
                default:
                    return false;
            }
        }
    }
}
