using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using csgo_creator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerAPINoFunction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        public IServerService service;

        public ServerController(IServerService service)
        {
            this.service = service;
        }

        [Route("[action]")]
        public async Task<IActionResult> Start()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var server = service.GetServer(ipAddress);
            await service.StartServer(server);
            service.Save();
            return new OkResult();
        }

        [Route("[action]")]
        public async Task<IActionResult> Stop()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var server = service.GetServer(ipAddress);
            await service.StopServer(server);
            service.Save();
            return new OkResult();
        }

        [Route("[action]/{command}")]
        public async Task<IActionResult> ExecuteCommand([FromRoute]string command)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var server = service.GetServer(ipAddress);
            await service.ExecuteServerCommand(server, command);
            service.Save();
            return new OkResult();
        }
    }
}