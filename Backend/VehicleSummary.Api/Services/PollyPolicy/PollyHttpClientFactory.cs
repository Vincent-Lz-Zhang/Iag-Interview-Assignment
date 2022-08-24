using Flurl.Http.Configuration;
using System.Net.Http;
using VehicleSummary.Api.Services.Interfaces;

namespace VehicleSummary.Api.Services.PollyPolicy
{
    public class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        private readonly IPollyPolicies _pollyPolicies;

        public PollyHttpClientFactory(IPollyPolicies pollyPolicies)
        {
            _pollyPolicies = pollyPolicies;
        }
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new PolicyHandler(_pollyPolicies)
            {
                InnerHandler = base.CreateMessageHandler()
            };
        }
    }
}
