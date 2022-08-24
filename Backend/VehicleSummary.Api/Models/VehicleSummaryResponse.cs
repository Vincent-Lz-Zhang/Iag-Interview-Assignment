using System.Collections.Generic;

namespace VehicleSummary.Api.Models
{
    public class VehicleSummaryResponse
    {
        public string Make { get; set; }
        public List<YearsAvailableOfVehicleModel> Models { get; set; } = new List<YearsAvailableOfVehicleModel>();
    }
}