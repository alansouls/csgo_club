using System;
using System.Collections.Generic;
using System.Text;

namespace CsgoClubEF.Entities
{
    public class FriendList : BaseEntity
    {
        public User User { get; set; }
        public User Friend { get; set; }

        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
    }
}
