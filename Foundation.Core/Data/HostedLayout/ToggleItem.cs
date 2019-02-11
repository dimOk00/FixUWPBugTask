using System;

namespace Foundation.Core.Data
{
    public class ToggleItem : InteractableItem
    {
        public Uri NormalState { get; set; }
        public Uri PressedState { get; set; }
        public Uri SelectedState { get; set; }
        public Uri DisabledState { get; set; }
        public string ToggleGroup { get; set; }
        public ToggleItemState DefaultState { get; set; } = ToggleItemState.Normal;
    }


    public enum ToggleItemState
    {
        Normal,
        Pressed,
        Selected,
        Disabled
    }
}
