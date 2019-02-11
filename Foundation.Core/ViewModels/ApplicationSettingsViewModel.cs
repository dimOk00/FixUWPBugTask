using Foundation.Core.Data;
using GhostCore.MVVM;
using GhostCore.UWP.AutoFormGeneration;

namespace Foundation.Core.ViewModels
{
    [FormGroupBehaviour(GroupInto = FormGroupMode.Tabs)]
    public class ApplicationSettingsViewModel : ContentItemViewModel
    {
        private ScreensaverSettingsViewModel _screensaverSettings;
        private FontSettingsViewModel _fontSettings;
        private MenuSettingsViewModel _menuSettings;
        private WorkspaceBackgroundViewModel _backgroundSettings;

        [ExpandClassAsGroup(GroupLabel = "Background Settings")]
        public WorkspaceBackgroundViewModel WorkspaceSettings
        {
            get { return _backgroundSettings; }
            set { _backgroundSettings = value; OnPropertyChanged(nameof(WorkspaceSettings)); }
        }

        [ExpandClassAsGroup(GroupLabel = "Font Settings")]
        public FontSettingsViewModel FontSettings
        {
            get { return _fontSettings; }
            set { _fontSettings = value; OnPropertyChanged(nameof(FontSettings)); }
        }

        [ExpandClassAsGroup(GroupLabel = "Menu Settings")]
        public MenuSettingsViewModel MenuSettings
        {
            get { return _menuSettings; }
            set { _menuSettings = value; OnPropertyChanged(nameof(MenuSettings)); }
        }

        [ExpandClassAsGroup(GroupLabel = "Screensaver Settings")]
        public ScreensaverSettingsViewModel ScreensaverSettings
        {
            get { return _screensaverSettings; }
            set { _screensaverSettings = value; OnPropertyChanged(nameof(ScreensaverSettings)); }
        }

        [HiddenFormItem]
        public new string DisplayLabel
        {
            get { return Model.DisplayLabel; }
            set { Model.DisplayLabel = value; OnPropertyChanged(nameof(DisplayLabel)); }
        }

        [HiddenFormItem]
        public new bool IsHidden
        {
            get { return Model.IsHidden; }
            set { Model.IsHidden = value; OnPropertyChanged(nameof(IsHidden)); }
        }


        public ApplicationSettingsViewModel(ApplicationSettings model)
             : base(model)
        {
            _fontSettings = new FontSettingsViewModel(model.FontSettings);
            _menuSettings = new MenuSettingsViewModel(model.MenuSettings);
            _backgroundSettings = new WorkspaceBackgroundViewModel(model.WorkspaceBackground);
            _screensaverSettings = new ScreensaverSettingsViewModel(model.ScreensaverSettings);
        }
    }

    public class ScreensaverSettingsViewModel : WorkspaceBackgroundViewModel
    {
        public double ActivationTime
        {
            get { return ModelAs<ScreensaverSettings>().ActivationTime; }
            set { ModelAs<ScreensaverSettings>().ActivationTime = value; OnPropertyChanged(nameof(ActivationTime)); }
        }

        public ScreensaverSettingsViewModel(ScreensaverSettings model) : base(model)
        {
        }
    }
}
