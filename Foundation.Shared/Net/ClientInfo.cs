using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Shared.Net
{
    public class ClientInfo
    {
        public string Label { get; set; }
        public string ClientIdentifier { get; set; }
        public HostType HostType { get; set; }
    }
    
    public enum HostType
    {
        TouchscreenWall,
        SimpleWall,
        Tablet
    }
}
