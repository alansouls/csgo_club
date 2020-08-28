using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;

namespace csgo_creator
{
    class Program
    {
        static void Main(string[] args)
        {
            try 
            {
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    // You can start any process, HelloWorld is a do-nothing example.
                    p.StartInfo.FileName = "/bin/bash";
                    p.StartInfo.Arguments = "commands/exec_server.sh";
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.Start();
                    Exit(p);
                    p.WaitForExit();
                }
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }

        public static async Task Exit(Process p) 
        {
            await Task.Delay(5000);
            p.StandardInput.WriteLine("exit");
        }
    }
}
