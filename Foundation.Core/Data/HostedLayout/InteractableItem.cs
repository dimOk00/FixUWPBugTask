using GhostCore.Math;
using System;
using System.Collections.Generic;

namespace Foundation.Core.Data
{
    public class InteractableItem : HostedLayoutItem
    {
        public int HighlightColor { get; set; }
        public HostedLayoutAction Action { get; set; }
        public ContentArea ContentAreaTarget { get; set; }
        public HostedLayoutPage PageTarget { get; set; }
        public ContentItem ItemTarget { get; set; }
        public bool IsNonStandardHitArea { get; set; }
        public List<Vector2D> Shape { get; set; }

        public string PageTargetName { get; set; }
    }

    public class ButtonItem : InteractableItem
    {
        public Uri ImagePath { get; set; }
        public bool UseImage { get; set; } = true;
        public string StyleKey { get; set; }
    }

    public enum HostedLayoutAction
    {
        NavigateToPage,
        OpenContentToWindow,
        OpenContentToArea,
        OpenPopup,
        CloseContent
    }
}
