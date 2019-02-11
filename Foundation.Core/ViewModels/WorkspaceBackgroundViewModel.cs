using Foundation.Core.ControlData;
using GhostCore.MVVM;
using GhostCore.UWP.AutoFormGeneration;
using System;

namespace Foundation.Core.ViewModels
{
    public class WorkspaceBackgroundViewModel : ViewModelBase<WorkspaceBackground>
    {
        public BackgroundType Type
        {
            get { return Model.Type; }
            set { Model.Type = value; OnPropertyChanged(nameof(Type)); }
        }

        [BrowseFormItem(Label = "Background Source")]
        public Uri BackgroundSource
        {
            get { return Model.BackgroundSource; }
            set { Model.BackgroundSource = value; OnPropertyChanged(nameof(BackgroundSource)); }
        }


        public bool EnableBlurringTransition
        {
            get { return Model.EnableBlurring; }
            set { Model.EnableBlurring = value; OnPropertyChanged(nameof(EnableBlurringTransition)); }
        }

        [ExpandClassAsGroup( GroupLabel = "Blur Parameter On")]
        public BlurParameter BlurParameterOn
        {
            get { return Model.BlurParameterOn; }
            set { Model.BlurParameterOn = value; OnPropertyChanged(nameof(BlurParameterOn)); }
        }

        [ExpandClassAsGroup(GroupLabel = "Blur Parameter Off")]
        public BlurParameter BlurParameterOff
        {
            get { return Model.BlurParameterOff; }
            set { Model.BlurParameterOff = value; OnPropertyChanged(nameof(BlurParameterOff)); }
        }

        public WorkspaceBackgroundViewModel(WorkspaceBackground model) : base(model)
        {
        }
    }
}
