using GhostCore.Math;
using System.Collections.Generic;

namespace Foundation.Core.Data
{
    public class ContentArea : HostedLayoutItem
    {
        public bool HasContent { get; set; }
        public ContentItem Content { get; set; }
        public bool IsPopup { get; set; }
        public bool IsLightDismiss { get; set; }
        public ContentAreaState State { get; set; }
        public List<Vector2D> Shape { get; set; }
    }
    public enum ContentAreaState
    {
        Opened,
        Closed
    }

}
