using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csgo_club_web_app.Models
{
    public class MatchModel
    {
        public bool IsLeader { get; set; }

        public GameMatch GameMatch { get; set; }
    }
}
