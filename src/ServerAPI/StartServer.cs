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
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] 
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
    }
}
