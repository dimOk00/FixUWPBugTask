using Foundation.Workspace.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Foundation.Workspace.Helpers
{
    public static class VisibilityBindingHelper
    {
        public static readonly DependencyProperty ContentPathProperty =
            DependencyProperty.RegisterAttached("ContentPath", typeof(string), typeof(VisibilityBindingHelper), new PropertyMetadata(null, BindingPathPropertyChanged));

        public static string GetContentPath(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentPathProperty);
        }

        public static void SetContentPath(DependencyObject obj, string value)
        {
            obj.SetValue(ContentPathProperty, value);
        }

        private static void BindingPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string propertyPath)
            {
                var binding = new Binding
                {
                    Path = new PropertyPath($"{propertyPath}"),
                    Converter = DeviceBasedMenuItemVisibilityConverter.SharedInstance,
                    Source = obj,
                    Mode = BindingMode.TwoWay,
                };

                BindingOperations.SetBinding(obj, UIElement.VisibilityProperty, binding);
            }
        }
    }
}
