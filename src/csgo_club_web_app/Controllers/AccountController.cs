using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace csgo_club_web_app.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnityOfWork _unityOfWork;

        public AccountController(IUnityOfWork unityOfWork, IConfiguration configuration)
        {
            _unityOfWork = unityOfWork;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IActionResult SignIn()
        {
            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return Challenge(new AuthenticationProperties { RedirectUri = "/Account/Check" }, "Steam");
        }

        public IActionResult Check()
        {
            var steamId = UInt64.Parse(User.Claims.First().Value.Split("id/")[2]);
            var user = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == steamId).FirstOrDefault();
            if(user == null)
            {
                var hasCsgo = CheckHasCsgo(steamId);
                if(!hasCsgo)
                    return Redirect("/Account/SignOut");
                var newUser = AddUser(steamId);
                AddFriends(steamId, newUser);
            }
            return Redirect("/Home/Index");
        }
        public async Task<IActionResult> SignOut()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });

            return Redirect("/Home/Index");
        }

        private User AddUser(ulong steamId)
        {
            var user = new User();
            var url = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" +
                Configuration["SteamApiKey"] + "&steamids=" + steamId.ToString();
            var httpClient = new HttpClient();
            var httpResponse = httpClient.GetAsync(url).Result;
            var userData = JObject.Parse(httpResponse.Content.ReadAsStringAsync().Result)["response"];
            var userPlayers = userData["players"].ToArray().First();
            user.Name = userPlayers["personaname"].Value<string>();
            user.ProfilePicture = userPlayers["avatarfull"].Value<string>();
            user.SteamId = steamId;
            _unityOfWork.GetRepository<User>().Add(user);
            _unityOfWork.Save();
            user = _unityOfWork.GetRepository<User>().GetByIdAsNoTracking(user.Id);
            return user;
        }
        private void AddFriends(ulong steamId, User user)
        {
            var url = "http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=" +
                Configuration["SteamApiKey"] + "&steamid=" + steamId.ToString() + "&relationship=friend";
            var httpClient = new HttpClient();
            var httpResponse = httpClient.GetAsync(url).Result;
            var friendsData = JObject.Parse(httpResponse.Content.ReadAsStringAsync().Result)["friendslist"];
            var friendListJson = friendsData["friends"].ToList();
            var friendList = new List<FriendList>();
            foreach(var f in friendListJson)
            {
                var friendSteamId = f["steamid"].Value<ulong>();
                var friend = _unityOfWork.GetRepository<User>().Query(x => x.SteamId == friendSteamId).FirstOrDefault();
                if(friend != null)
                    friendList.Add(new FriendList() { UserId = user.Id, FriendId = friend.Id });
            }
            _unityOfWork.GetRepository<FriendList>().AddMany(friendList);
            _unityOfWork.Save();
        }
        private bool CheckHasCsgo(ulong steamId)
        {
            try
            {
                var url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" +
                    Configuration["SteamApiKey"] + "&steamid=" + steamId.ToString() + "&format=json";
                var httpClient = new HttpClient();
                var httpResponse = httpClient.GetAsync(url).Result;
                var gamesData = JObject.Parse(httpResponse.Content.ReadAsStringAsync().Result)["response"];
                var gamesList = gamesData["games"].ToList();
                foreach (var g in gamesList)
                {
                    if (g["appid"].Value<int>() == 730)
                        return true;
                }
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}