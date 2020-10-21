using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using csgo_club_web_app.Models;
using csgo_club_web_app.Services;
using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csgo_club_web_app.Controllers
{
    public class LobbyController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;
        
        public LobbyController(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }
        public IActionResult Index()
        {
            var id = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == id)
                .Include(x => x.Matches).ThenInclude(x => x.GameMatch).FirstOrDefault();
            var match = user.Matches.ToList().Find(x => x.GameMatch.Status != MatchStatus.Finished);
            if (match != null)
                return Redirect(Url.Action("Match", new { id = match.GameMatchId }));

            var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(x => x.Status == MatchStatus.Lobby)
                .Include(x => x.Matches).ThenInclude(x => x.User).ToList();
            return View(gameMatch);
        }

        public IActionResult Match([FromRoute] Guid id)
        {
            var userId = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == userId)
                .Include(x => x.Matches).ThenInclude(x => x.GameMatch).FirstOrDefault();
            var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(x=> x.Id == id).Include(x=> x.Matches).ThenInclude(x=> x.User).Include(s => s.Server).FirstOrDefault();
            bool isLeader = gameMatch.Matches.Where(s => s.IsLeader).First().User.Id == user.Id;
            return View(new MatchModel
            {
                IsLeader = isLeader,
                GameMatch = gameMatch
            });
        }

        public IActionResult NoServer()
        {
            return View();
        }
        public IActionResult EnterLobby([FromRoute] Guid id)
        {
            var userId = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == userId)
                .Include(x=> x.Matches).ThenInclude(x=> x.GameMatch).FirstOrDefault();
            var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(x=> x.Id == id)
                .Include(x=> x.Matches).ThenInclude(x=> x.User).FirstOrDefault();
            var match = gameMatch.Matches.ToList().Find(x => x.UserId == user.Id);
            if (match != null)
                return Redirect(Url.Action("Match", new { id = gameMatch.Id }));

            match = user.Matches.ToList().Find(x => x.GameMatch.Status != MatchStatus.Finished);
            if (match != null)
                return Redirect(Url.Action("Match", new { id = match.GameMatchId }));
            _unityOfWork.GetRepository<PlayerToMatch>().Add(
            new PlayerToMatch
            {
                UserId = user.Id,
                GameMatchId = gameMatch.Id,
                IsLeader = false
            });
            _unityOfWork.Save();
            return Redirect(Url.Action("Match", new { id = gameMatch.Id }));
        }
        public IActionResult Create()
        {
            var id = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == id).FirstOrDefault();
            var server = _unityOfWork.GetRepository<Server>().Query(x => !x.Matches.Where(y => y.Status != MatchStatus.Finished).Any()).FirstOrDefault();
            var gmId = Guid.NewGuid();
            if(server == null)
            {
                return Redirect(Url.Action("NoServer"));
            }
            var lobby = new GameMatch()
            {
                Status = MatchStatus.Lobby,
                Id = gmId,
                ServerId = server.Id,
                Matches = new List<PlayerToMatch>()
                {
                    new PlayerToMatch() 
                    {
                        IsLeader = true,
                        UserId = user.Id,
                        GameMatchId = gmId
                    }
                },
                
            };
            _unityOfWork.GetRepository<GameMatch>().Add(lobby);
            _unityOfWork.Save();
            return Redirect(Url.Action("Match", new { id = gmId}));
        }

        public async Task<IActionResult> Start([FromRoute] Guid id, [FromQuery] string ip)
        {
            var server = _unityOfWork.GetRepository<Server>().Query(s => s.Ip == ip).First();
            var result = await server.StartServer();
            if (result != "")
            {
                var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(s => s.Id == id).First();
                gameMatch.Status = MatchStatus.Started;
                gameMatch.Password = result;
                gameMatch.MatchStartDate = DateTime.Now;
                server.IsOn = true;
                _unityOfWork.Save();
            }
            return Redirect(Url.Action("Match", new { id }));
        }

        public async Task<IActionResult> Stop([FromRoute] Guid id, [FromQuery] string ip)
        {
            var server = _unityOfWork.GetRepository<Server>().Query(s => s.Ip == ip).First();
            if (await server.StopServer())
            {
                var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(s => s.Id == id).First();
                gameMatch.Status = MatchStatus.Finished;
                gameMatch.MatchEndDate = DateTime.Now;
                server.IsOn = false;
                _unityOfWork.Save();
            }
            return Redirect(Url.Action("Index"));
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteCommand([FromQuery] string command, [FromQuery] string ip)
        {
            var server = _unityOfWork.GetRepository<Server>().Query(s => s.Ip == ip).First();
            if (await server.CommandServer(command))
                return Ok();
            return BadRequest();
        }

        public IActionResult FinishedMatches()
        {
            var id = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == id)
                .Include(u => u.Matches)
                .ThenInclude(m => m.GameMatch)
                .ThenInclude(g => g.Matches)
                .ThenInclude(m => m.User).FirstOrDefault();
            return View(user.Matches.Select(s => s.GameMatch).Where(s => s.Status == MatchStatus.Finished).ToList());
        }
    }
}
