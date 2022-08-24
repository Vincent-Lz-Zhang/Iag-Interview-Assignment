namespace VehicleSummary.Api.Models
{
    public class VehicleApiErrorResponse
    {
        public string Message { get; set; }

        public bool IsTransient { get; set; }
    }
}