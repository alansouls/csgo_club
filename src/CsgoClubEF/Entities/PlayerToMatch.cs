using System;
using System.Collections.Generic;
using System.Text;

namespace CsgoClubEF.Entities
{
    public class PlayerToMatch : BaseEntity
    {
        public User User { get; set; }
        public GameMatch GameMatch { get; set; }

        public Guid UserId { get; set; }
        public Guid GameMatchId { get; set; }
    }
}
