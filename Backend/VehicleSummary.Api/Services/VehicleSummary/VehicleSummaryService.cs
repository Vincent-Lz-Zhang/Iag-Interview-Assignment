using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Models;
using VehicleSummary.Api.Services.Interfaces;
using VehicleSummary.Api.ServicesInterfaces;

namespace VehicleSummary.Api.Services.VehicleSummary
{
    public class VehicleSummaryService : IVehicleSummaryService
    {
        private readonly IIagResilientApiClient _resilientApiClient;

        public VehicleSummaryService(IIagResilientApiClient resilientApiClient)
        {
            _resilientApiClient = resilientApiClient ?? throw new ArgumentNullException(nameof(resilientApiClient));
        }

        public async Task<VehicleSummaryResponse> GetSummaryByMake(string make, CancellationToken cancellationToken)
        {
            var response = new VehicleSummaryResponse { Make = make };

            var vehicleModelNameList = await GetVehicleModelsByMake(make, cancellationToken);

            foreach (var vehicleModelName in vehicleModelNameList)
            {
                if (!string.IsNullOrEmpty(vehicleModelName))
                {
                    var yearAvailableOfVehicleModelList = await GetYearsAvailableByVehicleModel(make, vehicleModelName, cancellationToken);
                    response.Models.AddRange(yearAvailableOfVehicleModelList);
                }
            }

            return response;
        }

        private async Task<List<string>> GetVehicleModelsByMake(string make, CancellationToken cancellationToken)
        {
            var modelNameList = await _resilientApiClient
                .GetVehicleModelNamesByMake(make, cancellationToken);

            return modelNameList;
        }

        private async Task<List<YearsAvailableOfVehicleModel>> GetYearsAvailableByVehicleModel(string make, string model, CancellationToken cancellationToken)
        {
            var yearsAvailableList = await _resilientApiClient
                .GetYearsAvailableByVehicleModel(make, model, cancellationToken);

            return yearsAvailableList.Select(y => new YearsAvailableOfVehicleModel { Name = model, YearsAvailable = y }).ToList();
        }

    }
}