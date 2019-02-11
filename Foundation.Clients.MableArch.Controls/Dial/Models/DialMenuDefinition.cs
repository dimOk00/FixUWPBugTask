using System.Collections.Generic;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class DialMenuDefinition
    {
        public double SubMenuBackgroundBlur { get; set; }
        public int HostedWidth { get; set; }
        public int HostedHeight { get; set; }
        public int MenuItemFontSize { get; set; }
        public int SubMenuFontSize { get; set; }
        public bool IsDialPressToCancelMediaEnabled { get; set; }
        public string IntroMedia { get; set; }
        public List<Section> Sections { get; set; }
    }
}
