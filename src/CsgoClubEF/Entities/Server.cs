using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsgoClubEF.Entities
{
    public class Server : BaseEntity
    {
        public bool IsOn { get; set; }

        public string Ip { get; set; }

        public IEnumerable<GameMatch> Matches { get; set; }

        [NotMapped]
        public Process ServerProcess { get; set; }

        [NotMapped]
        public IPAddress ServerIp { get; set; }

        [NotMapped]
        public Thread ProcessThread { get; set; }

        [NotMapped]
        public StreamWriter InputServerStream { get; set; }

        [NotMapped]
        public StreamReader OutputServerStream { get; set; }
    }
}
