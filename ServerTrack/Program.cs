using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerTrack.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTrack
{
    class Program
    {
        static void Main(string[] args)
        {
            var numFakeServers = 10;
            var servers = new List<string>();
            for (int x = 0; x < numFakeServers; x++)
            {
                servers.Add(Guid.NewGuid().ToString());
            }

            string baseAddress = "http://localhost:9000";

            Console.WriteLine("Starting host");

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.Write("Starting fake servers");
                var random = new Random();
                foreach (var server in servers)
                {
                    var t = Task.Run(() =>
                    {
                        var client = new HttpClient();
                        while (true)
                        {
                            var cpu = random.NextDouble() * 100;
                            var ram = random.NextDouble() * 100;
                            var url = $"{baseAddress}/{server}/{cpu}/{ram}";
                            var response = client.PostAsync(url, null).Result;
                            var result = response.Content.ReadAsStringAsync().Result;
                            //Console.WriteLine($"{url}: {result}");
                            Thread.Sleep(100 * random.Next(100));
                        }
                    });
                    Console.Write(".");
                }
                var t1 = Task.Run(() =>
                {
                    var client = new HttpClient();
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine($"Server name: {servers.First()}");
                        Console.WriteLine("Minutes:");
                        var response = client.GetAsync($"{baseAddress}/{servers.First()}/minutes").Result;
                        var resultString = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(resultString);
                        Console.WriteLine("Hours:");
                        response = client.GetAsync($"{baseAddress}/{servers.First()}/hours").Result;
                        resultString = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(resultString);
                        Thread.Sleep(1000);
                    }
                });
                Console.ReadLine();
            }
        }
    }
}
