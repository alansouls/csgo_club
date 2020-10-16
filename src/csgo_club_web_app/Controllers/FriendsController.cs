using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using csgo_club_web_app.Models;
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

        public IActionResult Users([FromQuery] string name)
        {
            var users = _unityOfWork.GetRepository<User>().Query(x => x.Name.Contains(name)).ToList();
            return View(users);
        }

        public IActionResult Add([FromRoute] Guid id)
        {
            var steamId = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == steamId)
                .Include(x=> x.Friends).ThenInclude(x=> x.Friend).FirstOrDefault();
            if(user.Friends.ToList().Find(x=> x.FriendId == id) != null)
                return Redirect(Url.Action("Index"));
            _unityOfWork.GetRepository<FriendList>().Add(new FriendList()
                {
                    UserId = user.Id,
                    FriendId = id
                });
            _unityOfWork.Save();
            return Redirect(Url.Action("Users"));
        }

        public IActionResult Search() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(SearchModel model)
        {
            if (ModelState.IsValid)
            {
                return Redirect(Url.Action("Users", new { name = model.Name }));
            }
            return Redirect(Url.Action("Search"));
        }
    }
}
