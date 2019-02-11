using GhostCore.MVVM;
using GhostCore.UWP.AutoFormGeneration;
using Foundation.Core.Data;
using System;

namespace Foundation.Core.ViewModels
{
    public class FontSettingsViewModel : ViewModelBase<FontSettings>
    {
        [BrowseFormItem(Label = "Primary Font Path")]
        public Uri PrimaryFontPath
        {
            get { return Model.PrimaryFontPath; }
            set { Model.PrimaryFontPath = value; OnPropertyChanged(nameof(PrimaryFontPath)); }
        }

        [BrowseFormItem(Label = "Secondary Font Path")]
        public Uri SecondaryFontPath
        {
            get { return Model.SecondaryFontPath; }
            set { Model.SecondaryFontPath = value; OnPropertyChanged(nameof(SecondaryFontPath)); }
        }

        [BrowseFormItem(Label = "Tertiary Font Path")]
        public Uri TertiaryFontPath
        {
            get { return Model.TertiaryFontPath; }
            set { Model.TertiaryFontPath = value; OnPropertyChanged(nameof(TertiaryFontPath)); }
        }

        [BrowseFormItem(Label = "Primary Icon Font Path")]
        public Uri PrimaryIconFontPath
        {
            get { return Model.PrimaryIconFontPath; }
            set { Model.PrimaryIconFontPath = value; OnPropertyChanged(nameof(PrimaryIconFontPath)); }
        }

        [BrowseFormItem(Label = "Secondary Icon Font Path")]
        public Uri SecondaryIconFontPath
        {
            get { return Model.SecondaryIconFontPath; }
            set { Model.SecondaryIconFontPath = value; OnPropertyChanged(nameof(SecondaryIconFontPath)); }
        }

        [BrowseFormItem(Label = "Tertiary Icon Font Path")]
        public Uri TertiaryIconFontPath
        {
            get { return Model.TertiaryIconFontPath; }
            set { Model.TertiaryIconFontPath = value; OnPropertyChanged(nameof(TertiaryIconFontPath)); }
        }
        public FontSettingsViewModel(FontSettings model) : base(model)
        {
        }

    }


}
