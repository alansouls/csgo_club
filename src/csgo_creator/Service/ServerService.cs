﻿using CsgoClubEF.Entities;
using CsgoClubEF.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csgo_creator.Service
{
    public class ServerService : IServerService
    {
        private readonly IRepository<Server> _repository;
        private readonly IUnityOfWork _unityOfWork;

        public ServerService(IUnityOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;
            _repository = unityOfWork.GetRepository<Server>();
        }

        public async Task<string> ExecuteServerCommand(Server server, string commandString)
        {
            await server.OutputServerStream.ReadLineAsync();
            await server.InputServerStream.WriteLineAsync(commandString);
            var response = await server.OutputServerStream.ReadLineAsync();
            return response;
        }

        public Server GetServer(IPAddress iPAddress)
        {
            return new Server
            {
                ServerIp = iPAddress,
                IsOn = false,
                Matches = Enumerable.Empty<GameMatch>(),
                ServerProcess = new Process(),
            };
        }

        public async Task<bool> StartServer(Server server)
        {
            var process = server.ServerProcess;
            var serverThread = new Thread(() => ExecuteServer(server));
            serverThread.Start();
            server.ProcessThread = serverThread;
            //Timeout To Start Server
            await Task.Delay(500);

            return server.IsOn;
        }

        public async Task<bool> StopServer(Server server)
        {
            if (!server.IsOn)
                return false;
            await server.InputServerStream.WriteLineAsync("exit");
            await Task.Delay(500);
            return server.IsOn;
        }

        private void ExecuteServer(Server server)
        {
            try
            {
                using var serverProcess = new Process();
                serverProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                serverProcess.StartInfo.FileName = "/bin/bash";
                serverProcess.StartInfo.Arguments = $"commands/exec_server.sh {server.Password}";
                serverProcess.StartInfo.CreateNoWindow = true;
                serverProcess.StartInfo.RedirectStandardInput = true;
                serverProcess.StartInfo.RedirectStandardOutput = true;
                serverProcess.Start();
                server.ServerProcess = serverProcess;
                server.IsOn = true;
                server.InputServerStream = serverProcess.StandardInput;
                server.OutputServerStream = serverProcess.StandardOutput;
                serverProcess.WaitForExit();
                server.IsOn = false;
                server.ServerProcess = null;
                server.InputServerStream = null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not open proccess");
            }
        }

        public void Save()
        {
            _unityOfWork.Save();
        }
    }
}
