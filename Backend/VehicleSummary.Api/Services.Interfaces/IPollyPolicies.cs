using Polly.Retry;
using System;
using System.Net.Http;

namespace VehicleSummary.Api.Services.Interfaces
{
    public interface IPollyPolicies
    {
        AsyncRetryPolicy<HttpResponseMessage> RetryPolicy { get; }

        public Func<int, TimeSpan> SleepDurationProvider { get; set; }
    }
}