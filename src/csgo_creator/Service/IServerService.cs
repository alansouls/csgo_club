
using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace csgo_creator
{
    public interface IServerService
    {
        /// <summary>
        /// Get server with requested IP
        /// </summary>
        /// <param name="iPAddress"></param>
        /// <returns></returns>
        Server GetServer(IPAddress iPAddress);

        /// <summary>
        /// Start the server, changing IsOn to true.
        /// </summary>
        /// <param name="server"></param>
        /// <returns>Returns IsOn</returns>
        Task<bool> StartServer(Server server);

        /// <summary>
        /// Execute a server command
        /// </summary>
        /// <param name="server"></param>
        /// <param name=""></param>
        /// <returns>Returns command's return or null if Error</returns>
        Task<string> ExecuteServerCommand(Server server, string commandString);

        /// <summary>
        /// Stop the server, changing IsOn to false.
        /// </summary>
        /// <param name="server"></param>
        /// <returns>Returns IsOn</returns>
        Task<bool> StopServer(Server server);
    }
}
