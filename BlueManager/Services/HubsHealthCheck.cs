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
            //var hubsHealth = new List<CheckReport>();
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
                            //  hubsHealth.Add(hub.IpAddress, true);
                            hubsHealth.Add(hub.IpAddress, new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, IsActive = hub.IsActive, Status = true });
                            // hubsHealth.Add(new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, Status = true });
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Hub {IP}({Location}) is not active at {Time}", hub.IpAddress, hub.LocationName, DateTime.Now);           
                        hubsHealth.Add(hub.IpAddress, new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, IsActive = hub.IsActive, Status = false });

                    }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Problem connection to server {IP}({Location}) at {Time}", hub.IpAddress, hub.LocationName, DateTime.Now);
                        //hubsHealth.Add(hub.IpAddress, false);
                        hubsHealth.Add(hub.IpAddress, new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, IsActive = hub.IsActive, Status = false });
                        //  hubsHealth.Add(new CheckReport() { IpAddress = hub.IpAddress, LocationName = hub.LocationName, Status = false });
                    }
                

            }

            return new HealthCheckResult(
                hubsHealth.Values.All(x => x.Status) ? HealthStatus.Healthy : HealthStatus.Unhealthy,
                "Description",
                null,
                hubsHealth.ToDictionary(k => k.Key, v => (object) v.Value)
            );
        }
    }
}