using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManager.Services.Model;
using BlueManagerPlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlueManager.Services
{
    public class ReportPollingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ReportPollingConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportPollingService(IServiceScopeFactory scopeFactory, IOptions<ReportPollingConfiguration> opt, IHttpClientFactory httpClientFactory)
        {
            _scopeFactory = scopeFactory;
            _configuration = opt.Value;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<BlueManagerContext>();

                    Console.WriteLine("[SearchingDevicesService] Service is Running" + DateTime.Now.ToString());

                    await using (context)
                    {
                        var hubs = await context.Hubs.ToListAsync(stoppingToken);
                        var reports = await GetReportsAsync(hubs);

                        foreach (var reportDownload in reports.Where(r=>r.IsSuccessful))
                        {
                            foreach (var tool in reportDownload.Report.Devices)
                            {
                                var updateTool = await context.Tools.Where(x => x.MacAddress == tool.MacAddress).FirstOrDefaultAsync(stoppingToken);
                                if (updateTool?.ToolName != null)
                                {
                                    var toolAtHub = new ToolAtHub()
                                    {
                                        Tool = updateTool,
                                        HubId = reportDownload.HubId,
                                        BleName = tool.Name,
                                        Timestamp = tool.Timestamp
                                    };

                                    await context.ToolAtHubs.AddAsync(toolAtHub, stoppingToken);
                                    await context.SaveChangesAsync(stoppingToken);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    await Task.Delay(_configuration.PollingInterval, stoppingToken);
                }
            }
        }

        private async Task<IEnumerable<ReportDownload>> GetReportsAsync(IEnumerable<Hub> hubs)
        {
            var reports = new ConcurrentBag<ReportDownload>();
            var downloads = hubs.Select(async h =>
            {
                using var http = _httpClientFactory.CreateClient();
                http.Timeout = TimeSpan.FromMilliseconds(_configuration.PollingRequestTimeout);
                var url = h.GetUrl();
                try
                {
                    var reportString = await http.GetStringAsync(url);
                    var report = JsonConvert.DeserializeObject<HubReport>(reportString);

                    reports.Add(new ReportDownload(h.ID, true, report));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error downloading report for HubId: {h.ID} from URL: {url}");
                    Console.Error.WriteLine(ex);
                    reports.Add(new ReportDownload(h.ID, false, null));
                }
            }).ToList();

            await Task.WhenAll(downloads);

            return reports.ToList();
        }
    }
}