using System;
using System.Collections.Generic;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class Section
    {
        public string Title { get; set; }
        public Uri BackgroundVideo { get; set; }
        public Uri TransitionToPreviousSectionVideo { get; set; }
        public Uri TransitionToNextSectionVideo { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }
}
