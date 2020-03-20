using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json.Linq;


namespace BlueManager.Services
{
    public class SearchingDevicesService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ReportPollingConfiguration _configuration;

        public SearchingDevicesService(IServiceScopeFactory scopeFactory, IOptions<ReportPollingConfiguration> opt)
        {
            _scopeFactory = scopeFactory;
            _configuration = opt.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _context = scope.ServiceProvider.GetRequiredService<BlueManagerContext>();

                        Console.WriteLine("[SearchingDevicesService] Service is Running" + DateTime.Now.ToString());

                        using (_context)
                        {
                            var hubs = await _context.Hubs.ToListAsync(stoppingToken);
                          //  var tools = await _context.Tools.ToListAsync(stoppingToken);
                            
                          var downloads = hubs.Select(async h =>
                            {
                                using var wc = new WebClient();
                                var report = await wc.DownloadStringTaskAsync($"http://{h.IpAddress}:8000/");
                                return (h.ID, report);
                            }).ToList();

                            await Task.WhenAll(downloads);

                            foreach (var reportTask in downloads)
                            {
                                var report = JsonConvert.DeserializeObject<HubReport>(reportTask.Result.report);
                                foreach (var tool in report.Devices)
                                {
                                    
                                  var updateTool = await _context.Tools.Where(x => x.MacAddress == tool.MacAddress).FirstOrDefaultAsync(stoppingToken);
                                    if (updateTool?.ToolName != null)
                                    {
                                        var toolAtHub = new ToolAtHub()
                                        {
                                            Tool = updateTool,
                                            HubId = reportTask.Result.ID,
                                            BleName = tool.Name,
                                            Timestamp = tool.Timestamp
                                        };

                                        await _context.ToolAtHubs.AddAsync(toolAtHub,stoppingToken);
                                        _context.SaveChanges();
                                    }
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

    }
}
