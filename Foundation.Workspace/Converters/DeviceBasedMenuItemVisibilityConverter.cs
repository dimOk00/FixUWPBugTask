using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using GhostCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Foundation.Workspace.Converters
{
    public class DeviceBasedMenuItemVisibilityConverter : IValueConverter
    {
        public static DeviceBasedMenuItemVisibilityConverter SharedInstance { get; private set; } = new DeviceBasedMenuItemVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ContentItemViewModel vm)
            {
                var htype = ServiceLocator.Instance.Resolve<RootConfiguration>().HostType;

                if (vm.IsHidden)
                    return Visibility.Collapsed;

                if (htype == vm.DisplayTarget)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
