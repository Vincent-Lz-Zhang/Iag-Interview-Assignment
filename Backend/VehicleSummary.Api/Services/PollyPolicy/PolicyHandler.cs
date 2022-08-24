using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Services.Interfaces;

namespace VehicleSummary.Api.Services.PollyPolicy
{
    public class PolicyHandler : DelegatingHandler
    {
        private readonly IPollyPolicies _pollyPolicies;

        public PolicyHandler(IPollyPolicies pollyPolicies)
        {
            _pollyPolicies = pollyPolicies;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _pollyPolicies.RetryPolicy
                .ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}
