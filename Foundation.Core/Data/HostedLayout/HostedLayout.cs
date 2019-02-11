using Foundation.Networking;
using GhostCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core.Data
{
    public class HostedLayout : ContentItem
    {

        [EventSerializerIgnore]
        public List<HostedLayoutPage> Pages { get; set; }

        public HostedLayout()
        {
            Pages = new List<HostedLayoutPage>();
        }
    }

    
}
