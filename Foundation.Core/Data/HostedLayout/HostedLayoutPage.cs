using System;
using System.Collections.Generic;

namespace Foundation.Core.Data
{
    public class HostedLayoutPage
    {
        public string Name { get; set; }
        public Uri BackgroundPath { get; set; }
        public int BackgroundColor { get; set; }
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }
        public List<HostedLayoutItem> Items { get; set; }
        public HostedLayout Parent { get; set; }

        public HostedLayoutPage(HostedLayout parent)
        {
            Parent = parent;
            Items = new List<HostedLayoutItem>();
        }
    }
}
