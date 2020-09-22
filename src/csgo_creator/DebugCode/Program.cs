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
            while (server.IsOn)
            {
                var command = Console.ReadLine();
                await service.ExecuteServerCommand(server, command);
            }
        }
    }
}
