namespace VehicleSummary.Api.Models
{
    public class VehicleApiOptions
    {
        public string IagApiBaseUrl { get; set; }
        public string GetModelsByMakeUrl { get; set; }
        public string GetModelsByMakeApiVersion { get; set; }
        public string GetYearsOfModelsByMakeUrl { get; set; }
        public string GetYearsOfModelsByMakeApiVersion { get; set; }
        public string ApiKeyHeaderName { get; set; }
        public string ApiKey { get; set; }
        public string ApiVersionParamName { get; set; }

    }
}
