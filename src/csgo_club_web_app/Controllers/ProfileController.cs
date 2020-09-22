using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlobServices.Services;
using csgo_club_web_app.Models;
using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace csgo_club_web_app.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IBlobService _blobService;

        public ProfileController(IUnityOfWork unityOfWork, IConfiguration configuration)
        {
            _unityOfWork = unityOfWork;
            _blobService = new BlobService(configuration["BlobConnection"]);
        }

        public IConfiguration Configuration { get; }

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
                var steamId = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
                var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == steamId).FirstOrDefault();
                user.Name = model.Name;
                if(model.PictureFile != null)
                {
                    if (model.PictureFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            model.PictureFile.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            _blobService.SelectBlobContainer(steamId.ToString());
                            var url = _blobService.UploadFile(fileBytes, model.PictureFile.FileName);
                            user.ProfilePicture = url;
                        }
                    }
                }
                _unityOfWork.Save();
                return Redirect("/Profile");
            }
            return View(model);
        }

    }
}
