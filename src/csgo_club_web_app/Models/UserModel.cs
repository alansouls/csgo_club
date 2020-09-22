using CsgoClubEF.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csgo_club_web_app.Models
{
    public class UserModel
    {
        public UserModel(User user)
        {
            Id = user.Id;
            SteamId = user.SteamId;
            Name = user.Name;
            ADR = user.ADR;
            KDR = user.KDR;
            ProfilePicture = user.ProfilePicture;
            Rank = user.Rank;
        }
        public Guid Id { get; set; }
        public UInt64 SteamId { get; set; }
        public string Name { get; set; }
        public decimal ADR { get; set; }
        public decimal KDR { get; set; }
        public string ProfilePicture { get; set; }
        public int Rank { get; set; }
        public IFormFile PictureFile { get; set; }
    }
}
