using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlueManager
{
    public class HubsHealthCheck : IHealthCheck
    {
        private BlueManagerContext _context;

        public HubsHealthCheck(BlueManagerContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var hubs = await _context.Hubs.ToListAsync(cancellationToken);

            var hubsHealth = new Dictionary<string, bool>();
            foreach (var hub in hubs)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        http.Timeout = TimeSpan.FromSeconds(2);
                        var response = await http.SendAsync(new HttpRequestMessage(HttpMethod.Head, hub.GetUrl()), cancellationToken);
                        response.EnsureSuccessStatusCode();
                        hubsHealth.Add(hub.IpAddress, true);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: log exception
                    hubsHealth.Add(hub.IpAddress, false);
                }
            }

            return new HealthCheckResult(
                hubsHealth.Values.All(x => x) ? HealthStatus.Healthy : HealthStatus.Unhealthy,
                "Description",
                null,
                hubsHealth.ToDictionary(k => k.Key, v => (object) v.Value)
            );
        }
    }
}