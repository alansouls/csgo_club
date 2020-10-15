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
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == id).Include(f=> f.FriendsTo).FirstOrDefault();
            var friendList = _unityOfWork.GetRepository<FriendList>().Query(x => x.UserId == user.Id).Include(x=> x.Friend).ToList();
            return View(user);
        }
    }
}
