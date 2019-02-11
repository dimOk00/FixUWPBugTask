using Foundation.Core.ControlData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foundation.Core.Data
{
    public class RootConfiguration
    {
        public HostType HostType { get; set; }
        public List<string> EnvironmentVariables { get; set; }
        public List<Session> Sessions { get; set; }
        public RemoteSettings RemoteSettings { get; set; }

        public RootConfiguration()
        {
            Sessions = new List<Session>();
            EnvironmentVariables = new List<string>();
            RemoteSettings = new RemoteSettings();
        }
    }

    public class RemoteSettings
    {
        public string ServerURL { get; set; }
    }

    public class GlobalSettings
    {
        public string RootFolder { get; set; }
    }

}
