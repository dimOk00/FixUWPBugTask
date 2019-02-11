using System;
using Foundation.Core.Data;
using GhostCore.MVVM;
using GhostCore.UWP.AutoFormGeneration;

namespace Foundation.Core.ViewModels
{
    public class WindowSettingsViewModel : ViewModelBase<WindowSettings>
    {
        [HiddenFormItem]
        public bool NotInheritProps => !InheritProperties;

        public bool InheritProperties
        {
            get { return Model.InheritProperties; }
            set
            {
                Model.InheritProperties = value;
                OnPropertyChanged(nameof(InheritProperties));
                OnPropertyChanged(nameof(NotInheritProps));
            }
        }

        /// <summary>
        /// Default value is 0.3 if not specified in the configuration
        /// </summary>
        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        public double MinWindowScale
        {
            get { return Model.MinWindowScale ?? 0.3; }
            set { Model.MinWindowScale = value; OnPropertyChanged(nameof(MinWindowScale)); }
        }

        /// <summary>
        /// Default value is double.MaxValue if not specified in the configuration
        /// </summary>
        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        public double MaxWindowScale
        {
            get { return Model.MaxWindowScale ?? double.MaxValue; }
            set { Model.MaxWindowScale = value; OnPropertyChanged(nameof(MaxWindowScale)); }
        }

        /// <summary>
        /// Default value is 940
        /// </summary>
        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        public double DefaultWidth
        {
            get { return Model.DefaultWidth ?? 940; }
            set { Model.DefaultWidth = value; OnPropertyChanged(nameof(DefaultWidth)); }
        }

        /// <summary>
        /// Default value is 540
        /// </summary>
        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        public double DefaultHeight
        {
            get { return Model.DefaultHeight ?? 540; }
            set { Model.DefaultHeight = value; OnPropertyChanged(nameof(DefaultHeight)); }
        }

        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        public bool ShrinkToContent
        {
            get { return Model.ShrinkToContent; }
            set { Model.ShrinkToContent = value; OnPropertyChanged(nameof(ShrinkToContent)); }
        }


        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        public bool HasDropShadow
        {
            get { return Model.HasDropShadow; }
            set { Model.HasDropShadow = value; OnPropertyChanged(nameof(HasDropShadow)); }
        }

        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        [SliderFormItem(Min = 0, Max = 1)]
        public double DropShadowOpacity
        {
            get { return Model.DropShadowOpacity; }
            set { Model.DropShadowOpacity = value; OnPropertyChanged(nameof(DropShadowOpacity)); }
        }

        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        [ColorPickerFormItem]
        public int DropShadowColor
        {
            get { return Model.DropShadowColor; }
            set { Model.DropShadowColor = value; OnPropertyChanged(nameof(DropShadowColor)); }
        }

        [VisibleIf(BooleanSourceProperty = nameof(NotInheritProps))]
        [ColorPickerFormItem]
        public int BackgroundColor
        {
            get { return Model.BackgroundColor; }
            set { Model.BackgroundColor = value; OnPropertyChanged(nameof(BackgroundColor)); }
        }

        public WindowSettingsViewModel(WindowSettings model) : base(model)
        {
        }

        public void InheritFrom(WindowSettingsViewModel parent)
        {
            InheritProperties = true;
            MinWindowScale = parent.MinWindowScale;
            MaxWindowScale = parent.MaxWindowScale;
            DefaultWidth = parent.DefaultWidth;
            DefaultHeight = parent.DefaultHeight;
            ShrinkToContent = parent.ShrinkToContent;
            HasDropShadow = parent.HasDropShadow;
            DropShadowOpacity = parent.DropShadowOpacity;
            DropShadowColor = parent.DropShadowColor;
            BackgroundColor = parent.BackgroundColor;
        }
    }
}
