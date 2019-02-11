using GhostCore.UWP.AutoFormGeneration;
using Newtonsoft.Json;
using System;

namespace Foundation.Core.Data
{
    public class MenuSettings
    {
        public Uri LogoPath { get; set; }
        public int TargetWidth { get; set; }

        public MenuItemColorState Normal { get; set; }
        public MenuItemColorState PointerOver { get; set; }
        public MenuItemColorState Pressed { get; set; }
        public MenuItemColorState Selected { get; set; }

        public MenuSettings()
        {
            Normal = new MenuItemColorState();
            PointerOver = new MenuItemColorState();
            Pressed = new MenuItemColorState();
            Selected = new MenuItemColorState();
        }
    }

    public class MenuItemColorState
    {
        [ColorPickerFormItem]
        public int Background { get; set; }

        [ColorPickerFormItem]
        public int Foreground { get; set; }
    }
}