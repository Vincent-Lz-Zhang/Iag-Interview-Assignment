using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleSummary.Api.Models;
using VehicleSummary.Api.ServicesInterfaces;

namespace VehicleSummary.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vehicle-checks")]
    public class VehicleChecksController : ControllerBase
    {
        private readonly IVehicleSummaryService _vehicleSummaryService;

        public VehicleChecksController(IVehicleSummaryService vehicleSummaryService)
        {
            _vehicleSummaryService = vehicleSummaryService ?? throw new ArgumentNullException(nameof(vehicleSummaryService));
        }

        [EnableCors(ApiConstants.AllowSpecificOrigin)]
        [HttpGet]
        [Route("makes/{make}")]
        public async Task<ActionResult<VehicleSummaryResponse>> Makes(string make, CancellationToken cancellationToken)
        {
            var result = await _vehicleSummaryService.GetSummaryByMake(make, cancellationToken);

            return Ok(result);
        }
    }
}