using System;
using System.Collections.Generic;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class MenuItem
    {
        public string Title { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public Uri Media { get; set; }
        public List<SubMenuItem> SubMenuItems { get; set; }
    }
}
