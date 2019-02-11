using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Foundation.Controls
{
    public sealed partial class AnnotationControl : UserControl
    {
        private InkPresenter _presenter;

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(AnnotationControl), new PropertyMetadata(1, OnScaleChanged));

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as AnnotationControl;
            var newval = (double)e.NewValue;
            that.transToolbarTransform.ScaleX = newval;
            that.transToolbarTransform.ScaleY = newval;
        }

        public AnnotationControl()
        {
            InitializeComponent();
            InitializeInkCanvas();
        }

        private void InitializeInkCanvas()
        {
            _presenter = icInkCanvas.InkPresenter;
            _presenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
        }

        public void HideToolbar()
        {
            itInkToolbar.Visibility = Visibility.Collapsed;
        }

        public void ShowToolbar()
        {
            itInkToolbar.Visibility = Visibility.Visible;
        }
    }
}
