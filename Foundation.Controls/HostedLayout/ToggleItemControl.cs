using Foundation.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Foundation.Controls.HostedLayout
{
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    public sealed class ToggleItemControl : Control
    {
        public Uri NormalStateUri
        {
            get { return (Uri)GetValue(NormalStateUriProperty); }
            set { SetValue(NormalStateUriProperty, value); }
        }

        public Uri PressedStateUri
        {
            get { return (Uri)GetValue(PressedStateUriProperty); }
            set { SetValue(PressedStateUriProperty, value); }
        }

        public Uri SelectedStateUri
        {
            get { return (Uri)GetValue(SelectedStateUriProperty); }
            set { SetValue(SelectedStateUriProperty, value); }
        }

        public Uri DisabledStateUri
        {
            get { return (Uri)GetValue(DisabledStateUriProperty); }
            set { SetValue(DisabledStateUriProperty, value); }
        }

        public ToggleItemState State
        {
            get { return (ToggleItemState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleItem), new PropertyMetadata(false, OnIsCheckedChanged));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(ToggleItemState), typeof(ToggleItemControl), new PropertyMetadata(ToggleItemState.Normal, OnStateChanged));

        public static readonly DependencyProperty DisabledStateUriProperty =
            DependencyProperty.Register("DisabledStateUri", typeof(Uri), typeof(ToggleItemControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedStateUriProperty =
            DependencyProperty.Register("SelectedStateUri", typeof(Uri), typeof(ToggleItemControl), new PropertyMetadata(null));

        public static readonly DependencyProperty PressedStateUriProperty =
            DependencyProperty.Register("PressedStateUri", typeof(Uri), typeof(ToggleItemControl), new PropertyMetadata(null));

        public static readonly DependencyProperty NormalStateUriProperty =
            DependencyProperty.Register("NormalStateUri", typeof(Uri), typeof(ToggleItemControl), new PropertyMetadata(null));

        public ToggleItemControl()
        {
            DefaultStyleKey = typeof(ToggleItemControl);

            PointerPressed += ToggleItemControl_PointerPressed;
            PointerReleased += ToggleItemControl_PointerReleased;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnStateChanged(State);
        }

        private void ToggleItemControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            IsChecked = !IsChecked;
        }

        private void ToggleItemControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            State = ToggleItemState.Pressed;
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as ToggleItemControl;
            that.OnStateChanged((ToggleItemState)e.NewValue);
        }
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as ToggleItemControl;
            that.OnIsCheckedChanged((bool)e.NewValue);
        }

        private void OnIsCheckedChanged(bool newValue)
        {
            if (newValue)
            {
                State = ToggleItemState.Selected;
            }
            else
            {
                State = ToggleItemState.Normal;
            }
        }

        private void OnStateChanged(ToggleItemState state)
        {
            switch (state)
            {
                case ToggleItemState.Normal:
                    VisualStateManager.GoToState(this, "Normal", false);
                    break;
                case ToggleItemState.Pressed:
                    VisualStateManager.GoToState(this, "Pressed", false);
                    break;
                case ToggleItemState.Selected:
                    VisualStateManager.GoToState(this, "Selected", false);
                    break;
                case ToggleItemState.Disabled:
                    VisualStateManager.GoToState(this, "Disabled", false);
                    break;
                default:
                    break;
            }
        }
    }
}
