using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Photobox.Web.HealthCheck;

public static class HealthCheck
{
    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
        .AddMySql(connectionString: configuration.GetConnectionString("PhotoboxConnectionString"))
        .AddCheck<MemoryHealthCheck>($"Photobox Service Memory Check", failureStatus: HealthStatus.Unhealthy, tags: ["Photobox Service"]);

        //services.AddHealthChecksUI(opt =>
        //{
        //    opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
        //    opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
        //    opt.SetApiMaxActiveRequests(1); //api requests concurrency    
        //    opt.AddHealthCheckEndpoint("photobox api", "https://localhost/api/health"); //map health check api    
        //})
        //.AddInMemoryStorage();
    }
}
