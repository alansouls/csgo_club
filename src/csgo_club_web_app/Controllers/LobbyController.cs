﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using csgo_club_web_app.Hubs;
using csgo_club_web_app.Models;
using csgo_club_web_app.Services;
using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace csgo_club_web_app.Controllers
{
    public class LobbyController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IHubContext<MatchHub> hubContext;

        public LobbyController(IUnityOfWork unityOfWork, IHubContext<MatchHub> hubContext)
        {
            _unityOfWork = unityOfWork;
            this.hubContext = hubContext;
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
            if (gameMatch == null)
            {
                return Redirect(Url.Action("LobbyNonExistent"));
            }
            else if (!gameMatch.Matches.Select(m => m.UserId).Contains(user.Id))
            {
                return Redirect(Url.Action("Index"));
            }
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
        public async Task<IActionResult> EnterLobby([FromRoute] Guid id)
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
            await hubContext.Clients.All.SendAsync("UpdateLobby", id);
            return Redirect(Url.Action("Match", new { id = gameMatch.Id }));
        }

        public async Task<IActionResult> Create()
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
            await hubContext.Clients.All.SendAsync("UpdateLobbyList");
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
            await hubContext.Clients.All.SendAsync("UpdateLobby", id);

            return Redirect(Url.Action("Match", new { id }));
        }

        public async Task<IActionResult> Stop([FromRoute] Guid id, [FromQuery] string ip)
        {
            var server = _unityOfWork.GetRepository<Server>().Query(s => s.Ip == ip).First();
            if (await server.StopServer())
            {
                var random = new Random();
                var gameMatch = _unityOfWork.GetRepository<GameMatch>().Query(s => s.Id == id)
                    .Include(s => s.Matches)
                    .ThenInclude(s => s.User).First();
                gameMatch.Status = MatchStatus.Finished;
                gameMatch.MatchEndDate = DateTime.Now;
                server.IsOn = false;
                var users = gameMatch.Matches.Select(s => s.User).ToList();
                users.ForEach(u =>
                {
                    if (u.KDR == 0)
                        u.KDR = (decimal)(random.NextDouble() * 2.5 + 0.5);
                    else
                    {
                        u.KDR += (decimal)(random.NextDouble() * 2.5 + 0.5);
                        u.KDR /= 2;
                    }
                    u.Rank = (int)Math.Round((u.KDR - 0.5m) / 0.25m, 0);
                });
                _unityOfWork.Save();
            }
            await hubContext.Clients.All.SendAsync("UpdateLobby", id);
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
            return View(user.Matches.Select(s => s.GameMatch).Where(s => s.Status == MatchStatus.Finished).OrderByDescending(x=> x.MatchEndDate).ToList());
        }

        public async Task<IActionResult> ExitLobby()
        {
            var id = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == id)
                .Include(u => u.Matches)
                .ThenInclude(m => m.GameMatch)
                .ThenInclude(g => g.Matches)
                .FirstOrDefault();
            var match = user.Matches.Where(s => s.GameMatch.Status == MatchStatus.Lobby).FirstOrDefault();
            if (!match.IsLeader)
            {
                _unityOfWork.GetRepository<PlayerToMatch>().Remove(match);
                _unityOfWork.Save();
            }
            else
            {
                var gameMatch = match.GameMatch;
                _unityOfWork.GetRepository<PlayerToMatch>().RemoveMany(gameMatch.Matches);
                _unityOfWork.GetRepository<GameMatch>().Remove(gameMatch);
                _unityOfWork.Save();
            }
            await hubContext.Clients.All.SendAsync("UpdateLobby", match.GameMatchId);
            await hubContext.Clients.All.SendAsync("UpdateLobbyList");
            return Redirect(Url.Action("Index"));
        }

        public IActionResult LobbyNonExistent()
        {
            return View();
        }
    }
}
