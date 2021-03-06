﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManager.Services.Model;
using BlueManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using BlueManager.Controllers;

namespace BlueManager.Services
{
    public class ReportPollingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ReportPollingConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReportPollingService> _logger;
        private readonly IMemoryCache _cache;


        public ReportPollingService(IServiceScopeFactory scopeFactory, IOptions<ReportPollingConfiguration> opt, IHttpClientFactory httpClientFactory, ILogger<ReportPollingService> logger, IMemoryCache memoryCache)
        //public ReportPollingService(IServiceScopeFactory scopeFactory, IOptions<ReportPollingConfiguration> opt, ILogger<ReportPollingService> logger, IMemoryCache memoryCache)
        {
            _scopeFactory = scopeFactory;
            _configuration = opt.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<BlueManagerContext>();
           

                    _logger.LogInformation("[SearchingDevicesService] Service is Running at" + DateTime.Now.ToString());

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
                                       // Tool = updateTool,
                                        ToolId = updateTool.Id,
                                        HubId = reportDownload.HubId,
                                        BleName = tool.Name,
                                        Timestamp = tool.Timestamp                         
                                    };

                                    var toolBattery = new ToolBatteryReadout()
                                    {
                                        ToolId = updateTool.Id,
                                        Timestamp = tool.BatteryTimestamp,
                                        BatteryState = tool.BatteryLevel

                                    };
                                    // var test = toolBattery;
                                    await context.ToolBatteryReadouts.AddAsync(toolBattery, stoppingToken);
                                    await context.SaveChangesAsync(stoppingToken);
                                    await context.ToolAtHubs.AddAsync(toolAtHub, stoppingToken);
                                    await context.SaveChangesAsync(stoppingToken);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "There was a problem with searching device service" + DateTime.Now.ToString());
                }
                finally
                {
                    await Task.Delay(_configuration.PollingInterval, stoppingToken);
                }
            }
        }

        private async Task<IEnumerable<ReportDownload>> GetReportsAsync(IEnumerable<Hub> hubs)
        {
            //using var scope = _scopeFactory.CreateScope();
           // var _httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var cacheEntry = new Dictionary<string, CheckReport> ();
            var reports = new ConcurrentBag<ReportDownload>();         
              var downloads = hubs.Select(async h =>
            {
                using var http = _httpClientFactory.CreateClient();
                http.Timeout = TimeSpan.FromMilliseconds(_configuration.PollingRequestTimeout);
                http.DefaultRequestHeaders.Add("timestamp", ((Int32)(DateTime.UtcNow.ToLocalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString());
                var url = h.GetUrl();
                try
                {
                    if (h.IsActive)
                    {
                        var reportString = await http.GetStringAsync(url);
                        var report = JsonConvert.DeserializeObject<HubReport>(reportString);
                        reports.Add(new ReportDownload(h.Id, true, report));
                        cacheEntry.Add(h.IpAddress, new CheckReport() { IpAddress = h.IpAddress, LocationName = h.LocationName, Status = true, IsActive = h.IsActive});
                    }
                    else
                    {
                        reports.Add(new ReportDownload(h.Id, false, null));
                        cacheEntry.Add(h.IpAddress, new CheckReport() { IpAddress = h.IpAddress, LocationName = h.LocationName, Status = false, IsActive = h.IsActive});
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error downloading report for HubId: {h.Id} from URL: {url}");
                    cacheEntry.Add(h.IpAddress, new CheckReport() { IpAddress = h.IpAddress, LocationName = h.LocationName, Status= false, IsActive = h.IsActive});
                    reports.Add(new ReportDownload(h.Id, false, null));
                }
            }).ToList();

            await Task.WhenAll(downloads);

              _cache.Remove(CacheKeys.Entry);
              _cache.Set(CacheKeys.Entry, cacheEntry);

            return reports.ToList();
        }
    }
}