using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace csgo_club_web_app.Services
{
    public static class ServerExtensions
    {
        public static async Task<bool> StartServer(this Server server)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{server.Ip}/api/server/start");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> StopServer(this Server server)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{server.Ip}/api/server/stop");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }


        public static async Task<bool> CommandServer(this Server server, string command)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{server.Ip}/api/server/executecommand/{command}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
    }
}
