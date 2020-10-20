using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using csgo_creator.Service;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace csgo_creator
{
    class Program
    {
        static void Main(string[] args)
        {
            var replays = Directory.GetFiles("C:\\Users\\maiaa\\", "*.mdf").ToList();
            replays.ForEach(r =>
            {
                var file = System.IO.File.ReadAllBytes(r);
                Console.WriteLine(r);
            });
        }
    }
}
