using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using csgo_creator.Service;
using System.Net;
using System.Threading;

namespace csgo_creator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServerService service = new ServerService();
            var ip = IPAddress.Parse("127.0.0.1");
            var server = service.GetServer(ip);
            await service.StartServer(server);
            await Task.Delay(1000);
            await service.StopServer(server);
        }
    }
}
