using Foundation.Core.Data;
using GhostCore.UWP.AutoFormGeneration;
using System;
using System.Linq;

namespace Foundation.Core.ViewModels
{
    public class ToggleItemViewModel : InteractableItemViewModel
    {
        private bool _isChecked;

        public override string ItemTitle => "Toggle Item";

        public Uri NormalState
        {
            get { return ModelAs<ToggleItem>().NormalState; }
            set { ModelAs<ToggleItem>().NormalState = value; OnPropertyChanged(nameof(NormalState)); }
        }

        public Uri PressedState
        {
            get { return ModelAs<ToggleItem>().PressedState; }
            set { ModelAs<ToggleItem>().PressedState = value; OnPropertyChanged(nameof(PressedState)); }
        }

        public Uri SelectedState
        {
            get { return ModelAs<ToggleItem>().SelectedState; }
            set { ModelAs<ToggleItem>().SelectedState = value; OnPropertyChanged(nameof(SelectedState)); }
        }

        public Uri DisabledState
        {
            get { return ModelAs<ToggleItem>().DisabledState; }
            set { ModelAs<ToggleItem>().DisabledState = value; OnPropertyChanged(nameof(DisabledState)); }
        }

        public ToggleItemState DefaultState
        {
            get { return ModelAs<ToggleItem>().DefaultState; }
            set { ModelAs<ToggleItem>().DefaultState = value; OnPropertyChanged(nameof(DefaultState)); }
        }


        public string ToggleGroup
        {
            get { return ModelAs<ToggleItem>().ToggleGroup; }
            set { ModelAs<ToggleItem>().ToggleGroup = value; OnPropertyChanged(nameof(ToggleGroup)); }
        }

        public Array ToggleItemStateSource => Enum.GetValues(typeof(ToggleItemState));

        [HiddenFormItem]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; OnPropertyChanged(nameof(IsChecked)); }
        }

        public ToggleItemViewModel(HostedLayoutItem model)
            : base(model)
        {

        }

        public override HostedLayoutItemViewModel Clone()
        {
            var oldContent = ModelAs<ToggleItem>();

            var newContent = new ToggleItem()
            {
                Height = oldContent.Height,
                Name = oldContent.Name,
                Shape = oldContent.Shape.ToList(),
                Width = oldContent.Width,
                X = oldContent.X,
                Y = oldContent.Y,
                Action = oldContent.Action,
                ContentAreaTarget = null,
                HighlightColor = oldContent.HighlightColor,
                IsNonStandardHitArea = oldContent.IsNonStandardHitArea,
                ItemTarget = oldContent.ItemTarget,
                PageTarget = oldContent.PageTarget,
                DefaultState = oldContent.DefaultState,
                DisabledState = oldContent.DisabledState,
                NormalState = oldContent.NormalState,
                PressedState = oldContent.PressedState,
                SelectedState = oldContent.SelectedState,
                ToggleGroup = oldContent.ToggleGroup
            };

            var newVm = new ToggleItemViewModel(newContent);

            return newVm;
        }
    }
}
