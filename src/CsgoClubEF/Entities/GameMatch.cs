using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace csgo_creator.Entities
{
    public class GameMatch : BaseEntity
    {
        public Server Server { get; set; }
        public Guid ServerId { get; set; }
    }
}
