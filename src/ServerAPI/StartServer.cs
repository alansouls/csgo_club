using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Autofac;
using csgo_creator;
using System.Net;
using System.Net.Http;

namespace ServerAPI
{
    [DependencyInjectionConfig(typeof(AutofacConfig))]
    public static class StartServer
    {
        [FunctionName("StartServer")]
        public static async Task<IActionResult> Start(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "start")] 
            HttpRequest req,
            [Inject]IServerService service,
            ILogger log)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var server = service.GetServer(ipAddress);
            await service.StartServer(server);
            service.Save();
            return new OkResult();
        }

        [FunctionName("StopServer")]
        public static async Task<IActionResult> Stop(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "stop")]
            HttpRequest req,
           [Inject]IServerService service,
           ILogger log)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var server = service.GetServer(ipAddress);
            await service.StopServer(server);
            service.Save();
            return new OkResult();
        }

        [FunctionName("ExecuteCommand")]
        public static async Task<IActionResult> ExecuteCommand(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "execute/{command}")]
            HttpRequest req,
           [Inject]IServerService service,
           ILogger log,
           string command)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var server = service.GetServer(ipAddress);
            await service.ExecuteServerCommand(server, command);
            service.Save();
            return new OkResult();
        }
    }
}
