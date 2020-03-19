//
//  IncomingEthTxService.cs
//
//  Author:
//       Nicholas Chen <nixholas@outlook.com>
//
//  Copyright (c) 2018 (c) Nicholas Chen
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BlueManager.Data;
using BlueManagerPlatform.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;


namespace BlueManager.Services
{
    public class SearchingDevicesService : BackgroundService
    {
        public SearchingDevicesService(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
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
                            List<Tool> tools = new List<Tool>();
                            IEnumerable<Hub> hubs = _context.Hubs.ToList();
                            foreach (var hub in hubs)
                            {

                                string url = "http://" + hub.IpAddress + ":8000/";
                                //string url = "file:///D:/location_template.json";
                                //string url = hub.IpAddress;
                                try
                                {
                                    using (var webClient = new WebClient())
                                    {
                                        var stringData = webClient.DownloadString(url);
                                        dynamic jsonObject = JObject.Parse(stringData);
                                        foreach (var tool in jsonObject.BLE_devices)
                                        {
                                            string _mac = tool.MAC;
                                            string _time = UnixTimeStampToDateTime(tool.time.ToString());
                                            string _name = tool.name;
                                            string _location = hub.LocationName;

                                            var rd = _context.Tools.Where(x => x.MacAddress == _mac).SingleOrDefault();
                                            if (rd != null && rd.ObjName != null)
                                            {
                                                var updateTool = _context.Tools.Find(rd.Id);
                                                updateTool.Location = _location;
                                                updateTool.Time = _time;
                                                updateTool.Name = _name;
                                                _context.Update(updateTool);
                                                _context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Problem z połączeniem z serwerem od adresie: " + hub.IpAddress);
                                }

                            }

                        }
                        // Run something

                        await Task.Delay(5000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static string UnixTimeStampToDateTime(string unixTimeStamp)
        {
            double unix = Convert.ToDouble(unixTimeStamp);
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            if (unixTimeStamp.Length == 10)
                dtDateTime = dtDateTime.AddSeconds(unix).ToLocalTime();
            else
                dtDateTime = dtDateTime.AddSeconds(1577836800).ToLocalTime();       // Unix timestamp is seconds past epoch

            return dtDateTime.ToString();
        }

    }
}
