using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csgo_club_web_app.Controllers
{
    public class FriendsController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;

        public FriendsController(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }

        public IActionResult Index()
        {
            var id = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == id).FirstOrDefault();
            var friends = _unityOfWork.GetRepository<FriendList>().Query(x => x.UserId == user.Id).Include(x=> x.Friend).ToList();
            user.Friends = friends;
            if (friends.Count() == 0)
                return View("NotFound");
            return View(user);
        }
    }
}
