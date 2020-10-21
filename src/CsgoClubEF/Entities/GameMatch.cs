using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CsgoClubEF.Entities
{
    public enum MatchStatus
    {
        Lobby,
        Started,
        Paused,
        Finished
    }
    public class GameMatch : BaseEntity
    {
        public Server Server { get; set; }
        public Guid ServerId { get; set; }
        public DateTime MatchStartDate { get; set; }
        public DateTime MatchEndDate { get; set; }

        public string FinalScore { get; set; }
        public string DemoUrl { get; set; }
        public MatchStatus Status { get; set; }
        public string MapName { get; set; }

        public string Password { get; set; }

        public IEnumerable<PlayerToMatch> Matches { get; set; }

        [NotMapped]
        public User Leader { get => Matches.Where(s => s.IsLeader).Select(s => s.User).FirstOrDefault(); }

    }
}
