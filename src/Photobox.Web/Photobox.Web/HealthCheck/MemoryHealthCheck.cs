﻿using Humanizer.Bytes;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Photobox.Web.HealthCheck;

public class MemoryHealthCheck(IOptionsMonitor<MemoryCheckOptions> options) : IHealthCheck
{
    public string Name => "memory_check";

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var options1 = options.Get(context.Registration.Name);

        // Include GC information in the reported diagnostics.
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var data = new Dictionary<string, object>()
        {
            { "AllocatedBytes", allocated },
            { "Gen0Collections", GC.CollectionCount(0) },
            { "Gen1Collections", GC.CollectionCount(1) },
            { "Gen2Collections", GC.CollectionCount(2) },
        };
        var status = allocated < options1.Threshold ? HealthStatus.Healthy : HealthStatus.Unhealthy;

        return Task.FromResult(
            new HealthCheckResult(
                status,
                description: "Reports degraded status if allocated bytes "
                    + $">= {options1.Threshold} bytes.",
                exception: null,
                data: data
            )
        );
    }
}

public class MemoryCheckOptions
{
    public string? Memorystatus { get; set; }

    public long Threshold { get; set; } = (long)ByteSize.FromGigabytes(1).Bytes;
}
