using System;
using System.Collections.Generic;
using System.Text;

namespace CsgoClubEF.Entities
{
    public class User : BaseEntity
    {
        public UInt64 SteamId { get; set; }

        public string Name { get; set; }

        public decimal ADR { get; set; }

        public decimal KDR { get; set; }

        public string ProfilePicture { get; set; }

        public int Rank { get; set; }

        public IEnumerable<PlayerToMatch> Matches { get; set; }
        public IEnumerable<FriendList> Friends { get; set; }
    }
}
