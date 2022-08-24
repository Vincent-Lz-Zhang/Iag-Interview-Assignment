using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Models;

namespace VehicleSummary.Api.ServicesInterfaces
{
    public interface IVehicleSummaryService
    {
        Task<VehicleSummaryResponse> GetSummaryByMake(string make, CancellationToken cancellationToken);
    }
}
