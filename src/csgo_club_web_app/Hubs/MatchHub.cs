using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csgo_club_web_app.Hubs
{
    public class MatchHub : Hub
    {
        public async Task SendUpdateLobby(Guid matchId)
        {
            await Clients.All.SendAsync("UpdateLobby", matchId);
        }
    }
}
