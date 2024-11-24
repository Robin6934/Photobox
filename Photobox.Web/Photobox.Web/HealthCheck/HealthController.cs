using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace Photobox.Web.HealthCheck;

[ApiController]
[Route("api/[controller]")]
public class HealthController(HealthCheckService healthCheckService) : Controller
{
    private readonly HealthCheckService healthCheckService = healthCheckService;

    /// <summary>
    /// Get Health
    /// </summary>
    /// <remarks>Provides an indication about the health of the API</remarks>
    /// <response code="200">API is healthy</response>
    /// <response code="503">API is unhealthy or in degraded state</response>
    [HttpGet]
    [ProducesResponseType<HealthReport>((int)HttpStatusCode.OK)]
    [ProducesResponseType<HealthReport>((int)HttpStatusCode.ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        var report = await healthCheckService.CheckHealthAsync();

        return report.Status == HealthStatus.Healthy ? Ok(report) : StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
    }
}
