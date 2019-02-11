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

namespace Foundation.Controls
{
    [TemplatePart(Name = "PART_LeftIndicator", Type = typeof(Button))]
    [TemplatePart(Name = "PART_RightIndicator", Type = typeof(Button))]
    public sealed class IndicatedHorizontalListView : ListView
    {
        public IndicatedHorizontalListView()
        {
            DefaultStyleKey = typeof(IndicatedHorizontalListView);
        }

        public ScrollViewer ScrollViewer { get; private set; }
        public Button PART_LeftIndicator { get; private set; }
        public Button PART_RightIndicator { get; private set; }

        public int IndicatorThreshold { get; set; } = 200;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            PART_LeftIndicator = GetTemplateChild("PART_LeftIndicator") as Button;
            PART_RightIndicator = GetTemplateChild("PART_RightIndicator") as Button;

            Loaded += IndicatedHorizontalListView_Loaded;
        }

        private void IndicatedHorizontalListView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= IndicatedHorizontalListView_Loaded;
            ScrollViewer.ViewChanging += ScrollViewer_ViewChanging;

            var hoff = ScrollViewer.HorizontalOffset;
            var vw = ScrollViewer.ViewportWidth;
            var ew = ScrollViewer.ExtentWidth;

            ProcIndicatorButtons(hoff, vw, ew);
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            var hoff = e.FinalView.HorizontalOffset;
            var vw = ScrollViewer.ViewportWidth;
            var ew = ScrollViewer.ExtentWidth;

            ProcIndicatorButtons(hoff, vw, ew);
        }

        private void ProcIndicatorButtons(double hoff, double vw, double ew)
        {
            var l = hoff >= IndicatorThreshold;
            PART_LeftIndicator.Visibility = l ? Visibility.Visible : Visibility.Collapsed;

            var r = (hoff + vw) < ew - IndicatorThreshold;
            PART_RightIndicator.Visibility = r ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
