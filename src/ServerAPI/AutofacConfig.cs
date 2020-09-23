using Autofac;
using AzureFunctions.Autofac.Configuration;
using csgo_creator;
using csgo_creator.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerAPI
{
    public class AutofacConfig
    {
        public AutofacConfig(string functionName)
        {
            DependencyInjection.Initialize(builder => {
                builder.RegisterType<ServerService>().As<IServerService>();
            }, functionName);
        }
    }
}
