using Foundation.Core.ControlData;
using Foundation.Core.Data;
using System;

namespace Foundation.Core.Data
{
    public class ApplicationSettings : ContentItem
    {
        public FontSettings FontSettings { get; set; }
        public MenuSettings MenuSettings { get; set; }
        public WorkspaceBackground WorkspaceBackground { get; set; }
        public ScreensaverSettings ScreensaverSettings { get; set; }

        public ApplicationSettings()
        {
            FontSettings = new FontSettings();
            MenuSettings = new MenuSettings();
            WorkspaceBackground = new WorkspaceBackground();
            ScreensaverSettings = new ScreensaverSettings();
        }
    }
}
