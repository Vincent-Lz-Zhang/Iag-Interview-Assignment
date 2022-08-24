using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VehicleSummary.Api.Services.Interfaces
{
    public interface IIagResilientApiClient
    {
        Task<List<string>> GetVehicleModelNamesByMake(string make, CancellationToken cancellationToken);
        Task<List<int>> GetYearsAvailableByVehicleModel(string make, string model, CancellationToken cancellationToken);
    }
}