using CsgoClubEF.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace csgo_club_web_app.Services
{
    public static class ServerExtensions
    {
        public static async Task<string> StartServer(this Server server)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://{server.Ip}:5000/api/server/start");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var password = JObject.Parse(result)["password"].Value<string>();
                return password;
            }
            return "";
        }

        public static async Task<bool> StopServer(this Server server)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://{server.Ip}:5000/api/server/stop");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }


        public static async Task<bool> CommandServer(this Server server, string command)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://{server.Ip}:5000/api/server/executecommand/{command}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
    }
}
