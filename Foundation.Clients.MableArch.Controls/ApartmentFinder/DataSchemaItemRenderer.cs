using Foundation.Data;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Foundation.Clients.MableArch.Controls.ApartmentFinder
{
    public sealed class DataSchemaItemRenderer : Control
    {
        private bool _templateApplied;
        private FrameworkElement _controlObject;

        public Grid PART_ControlPanel { get; set; }
        public ContentControl PART_TitlePresenter { get; set; }
        public event EventHandler<object> ValueChanged;

        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }

        public DataSchemaItem Schema
        {
            get { return (DataSchemaItem)GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(DataSchemaItemRenderer), new PropertyMetadata(null, OnValueChanged));

        public static readonly DependencyProperty SchemaProperty =
            DependencyProperty.Register("Schema", typeof(DataSchemaItem), typeof(DataSchemaItemRenderer), new PropertyMetadata(null, OnSchemaChanged));

        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(DataSchemaItemRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(DataSchemaItemRenderer), new PropertyMetadata(null));

        public DataSchemaItemRenderer()
        {
            DefaultStyleKey = typeof(DataSchemaItemRenderer);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            PART_ControlPanel = GetTemplateChild(nameof(PART_ControlPanel)) as Grid;
            PART_TitlePresenter = GetTemplateChild(nameof(PART_TitlePresenter)) as ContentControl;
            _templateApplied = true;

            OnSchemaChanged(Schema);
        }

        private static void OnSchemaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as DataSchemaItemRenderer;
            that.OnSchemaChanged(e.NewValue as DataSchemaItem);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as DataSchemaItemRenderer;
            that.OnValueChanged(e.NewValue);
        }

        private void OnValueChanged(object newValue)
        {
            if (!_templateApplied || Schema == null || _controlObject == null)
                return;

            if (Schema.DataType == DataType.FromSource)
            {
                var lv = _controlObject as ListView;
                if (newValue is IEnumerable<DataSourceItem> ie)
                {
                    foreach (var x in ie)
                    {
                        var actualItem = Schema.DataSource.FirstOrDefault(q => q.Value == x.Value);
                        lv.SelectedItems.Add(actualItem);
                    }
                }
            }

            if (Schema.DataType == DataType.Boolean)
            {
                if (newValue is bool b)
                {
                    (_controlObject as ToggleButton).IsChecked = b;
                }
            }

            if (Schema.DataType == DataType.Range)
            {
                var slider = (_controlObject as RangeSelector);
                if (newValue is ValueTuple<double, double> range)
                {
                    slider.RangeMin = range.Item1;
                    slider.RangeMin = range.Item2;
                }
            }
        }

        private void OnSchemaChanged(DataSchemaItem dataSchemaItem)
        {
            if (!_templateApplied || dataSchemaItem == null)
                return;

            PART_ControlPanel.Children.Clear();

            Title = dataSchemaItem.PropertyLabel;

            switch (dataSchemaItem.DataType)
            {
                case DataType.FromSource:
                    var lv = new ListView
                    {
                        ItemsSource = dataSchemaItem.DataSource,
                        SelectionMode = dataSchemaItem.IsMultiSelect ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single,
                        ItemsPanel = PART_ControlPanel.Resources["2RowUniformGridPanelTemplate"] as ItemsPanelTemplate
                    };

                    lv.SelectionChanged += DataSource_SelectionChanged;
                    _controlObject = lv;

                    break;
                case DataType.Range:

                    var slider = new RangeSelector
                    {
                        Minimum = dataSchemaItem.Min,
                        Maximum = dataSchemaItem.Max,
                    };

                    slider.ValueChanged += Slider_ValueChanged;

                    _controlObject = slider;

                    break;
                case DataType.Boolean:

                    var toggle = new ToggleButton
                    {
                        Content = dataSchemaItem.PropertyLabel
                    };

                    PART_TitlePresenter.Visibility = Visibility.Collapsed;

                    toggle.Checked += Toggle_CheckedChanged;
                    toggle.Unchecked += Toggle_CheckedChanged;

                    _controlObject = toggle;
                    break;
                default:
                    break;
            }

            PART_ControlPanel.Children.Add(_controlObject);
        }

        private void Toggle_CheckedChanged(object sender, RoutedEventArgs e)
        {
            //Value = (sender as ToggleButton).IsChecked;
            //ValueChanged(this, Value);
        }

        private void Slider_ValueChanged(object sender, RangeChangedEventArgs e)
        {
            //var slider = (sender as RangeSelector);
            //Value = (Min : slider.RangeMin, Max : slider.RangeMax);
            //ValueChanged(this, Value);
        }

        private void DataSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var lv = (sender as ListView);
            //Value = lv.SelectedItems;
            //ValueChanged(this, Value);
        }

        private void OnValueChangedEventTrigger(object value)
        {
            ValueChanged?.Invoke(this, value);
        }

    }
}
