using System;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class SubMenuItem
    {
        public static readonly SubMenuItem Back = new SubMenuItem { Title = "Back" };
        public string Title { get; set; }
        public Uri Media { get; set; }
    }
}
