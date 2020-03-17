using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManager.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlueManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 })
               .ConfigureServices((hostContext, services) =>
               {
                   // #region snippet3
                   //  services.AddSingleton<MonitorLoop>();
                   services.AddSingleton<IHostedService, SearchingDevicesService>();
                   //     services.AddHostedService<QueuedHostedService>();
                   //      services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                   //  #endregion

                   // #region snippet1
                   //services.AddHostedService<TimedHostedService>();
                   //  #endregion

                   #region snippet2
                   // services.AddHostedService<ConsumeScopedServiceHostedService>();
                   //     services.AddScoped<BlueManagerContext>();
                   // services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
                   #endregion
               })
                .Build())
            {
                // Start the host
                await host.StartAsync();

                // Wait for the host to shutdown
                await host.WaitForShutdownAsync();
            }


        }
    }
}









//public class Program
//{
//    public static void Main(string[] args)
//    {
//        CreateHostBuilder(args).Build().Run();
//    }

//    public static IHostBuilder CreateHostBuilder(string[] args) =>
//        Host.CreateDefaultBuilder(args)
//            .ConfigureWebHostDefaults(webBuilder =>
//            {
//                webBuilder.UseStartup<Startup>();

//            })


//        ;
//}