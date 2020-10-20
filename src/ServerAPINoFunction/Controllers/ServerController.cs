using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlobServices.Services;
using csgo_creator;
using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAPI;

namespace ServerAPINoFunction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        public IServerService service;
        public IBlobService blobService;
        public IUnityOfWork unityOfWork;

        public ServerController(IServerService service, IBlobService blobService, IUnityOfWork unityOfWork)
        {
            this.service = service;
            this.blobService = blobService;
            this.unityOfWork = unityOfWork;
        }

        [Route("[action]")]
        public async Task<IActionResult> Start()
        {
            var server = ServerInstance.Server;
            server.Password = ConstructPassword();
            await service.StartServer(server);
            service.Save();
            return new JsonResult(new { password = server.Password });
        }

        [Route("[action]")]
        public async Task<IActionResult> Stop()
        {
            var server = ServerInstance.Server;
            await service.StopServer(server);
            var replaysUrl = GetReplaysURL();
            var gameMatch = unityOfWork.GetRepository<GameMatch>().Query(s => s.Server.Ip == server.Ip && s.Status == MatchStatus.Started).First();
            gameMatch.DemoUrl = replaysUrl.Last();
            service.Save();
            return new OkResult();
        }

        [Route("[action]/{command}")]
        public async Task<IActionResult> ExecuteCommand([FromRoute]string command)
        {
            var server = ServerInstance.Server;
            Console.WriteLine(server?.GetHashCode());
            Console.WriteLine(server.InputServerStream?.GetHashCode());
            await service.ExecuteServerCommand(server, command);
            service.Save();
            return new OkResult();
        }


        private string ConstructPassword()
        {
            var random = new Random();
            var result = "";
            for (int i = 0; i < 10; ++i)
            {
                char[] selector = new char[3] ;
                selector[0] = (char)random.Next('0', '9');
                selector[1] = (char)random.Next('A', 'Z');
                selector[2] = (char)random.Next('a', 'z');
                result += selector[random.Next(0, 3)];
            }
            return result;
        }

        public IEnumerable<string> GetReplaysURL()
        {
            var result = new List<string>();
            var replays = Directory.GetFiles("/home/alan/csgo/csgo/", "*.dem").ToList();
            replays.ForEach(r =>
            {
                var file = System.IO.File.ReadAllBytes(r);
                blobService.SelectBlobContainer("replays");
                result.Add(blobService.UploadFile(file, r.Split("/").Last()));
            });

            return result;
        }
    }
}