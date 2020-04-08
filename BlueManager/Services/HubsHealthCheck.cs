using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManager.Services.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BlueManager
{
    public class HubsHealthCheck : IHealthCheck
    {
        private BlueManagerContext _context;
        private readonly ILogger<HubsHealthCheck> _logger;

        public HubsHealthCheck(BlueManagerContext context, ILogger<HubsHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var hubs = await _context.Hubs.ToListAsync(cancellationToken);
            var hubsHealth = new Dictionary<string, CheckReport>();

            foreach (var hub in hubs)
            {
                try
                {
                    if (hub.IsActive)
                    {
                        using (var http = new HttpClient())
                        {
                            http.Timeout = TimeSpan.FromSeconds(2);
                            var response = await http.SendAsync(new HttpRequestMessage(HttpMethod.Head, hub.GetUrl()), cancellationToken);
                            response.EnsureSuccessStatusCode();
                            hubsHealth.Add(hub.IpAddress, new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, IsActive = hub.IsActive, HealthStatus = HealthStatus.Healthy });
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Hub {IP}({Location}) is not active at {Time}", hub.IpAddress, hub.LocationName, DateTime.Now);
                        hubsHealth.Add(hub.IpAddress, new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, IsActive = hub.IsActive, HealthStatus = HealthStatus.Degraded });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Problem connection to server {IP}({Location}) at {Time}", hub.IpAddress, hub.LocationName, DateTime.Now);
                    hubsHealth.Add(hub.IpAddress, new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, IsActive = hub.IsActive, HealthStatus = HealthStatus.Unhealthy });
                }
            }
            return new HealthCheckResult(
                hubsHealth.OrderBy(x => x.Value.HealthStatus).FirstOrDefault().Value.HealthStatus,
                "Some problem conntecting to servers",
                null,
                hubsHealth.ToDictionary(k => k.Key, v => (object)v.Value)
            );
        }
    }
}