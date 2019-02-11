using GhostCore.MVVM;
using Foundation.Core.Data;
using GhostCore.UWP.AutoFormGeneration;
using System;

namespace Foundation.Core.ViewModels
{
    public class MenuSettingsViewModel : ViewModelBase<MenuSettings>
    {
        [BrowseFormItem(Label = "Logo Path")]
        public Uri LogoPath
        {
            get { return Model.LogoPath; }
            set { Model.LogoPath = value; OnPropertyChanged(nameof(LogoPath)); }
        }

        public int TargetWidth
        {
            get { return Model.TargetWidth; }
            set { Model.TargetWidth = value; OnPropertyChanged(nameof(TargetWidth)); }
        }


        [ExpandClassAsGroup(GroupLabel = "Normal State")]
        public MenuItemColorState Normal
        {
            get { return Model.Normal; }
            set { Model.Normal = value; OnPropertyChanged(nameof(Normal)); }
        }


        [ExpandClassAsGroup(GroupLabel = "Pointer Over State")]
        public MenuItemColorState PointerOver
        {
            get { return Model.PointerOver; }
            set { Model.PointerOver = value; OnPropertyChanged(nameof(PointerOver)); }
        }


        [ExpandClassAsGroup(GroupLabel = "Pressed State")]
        public MenuItemColorState Pressed
        {
            get { return Model.Pressed; }
            set { Model.Pressed = value; OnPropertyChanged(nameof(Pressed)); }
        }


        [ExpandClassAsGroup(GroupLabel = "Selected State")]
        public MenuItemColorState Selected
        {
            get { return Model.Selected; }
            set { Model.Selected = value; OnPropertyChanged(nameof(Selected)); }
        }

        public MenuSettingsViewModel(MenuSettings model) : base(model)
        {
        }

    }
}
