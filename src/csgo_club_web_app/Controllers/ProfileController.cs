using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using csgo_club_web_app.Models;
using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace csgo_club_web_app.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;

        public ProfileController(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
        }

        public ActionResult Index([FromRoute] UInt64 id)
        {
            if(id == 0)
                id = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == id).FirstOrDefault();
            if(user == null)
                return View("NotFound");
            var model = new UserModel(user);
            return View(model);
        }

        public ActionResult Edit()
        {
            var steamId = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == steamId).FirstOrDefault();
            var model = new UserModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                return View("Obrigado", model);
            }
            return View(model);
        }

    }
}
