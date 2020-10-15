using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(x => x.Status == MatchStatus.Lobby)
                .Include(x => x.Matches).ThenInclude(x => x.User).ToList();
            return View(gameMatch);
        }

        public IActionResult Match([FromRoute] Guid id)
        {
            var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(x=> x.Id == id).Include(x=> x.Matches).ThenInclude(x=> x.User).FirstOrDefault();
            return View(gameMatch);
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

            match = user.Matches.ToList().Find(x => x.GameMatch.Status == MatchStatus.Lobby);
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
    }
}
