using Foundation.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Foundation.Workspace
{
    public class WorkspaceStyleConverter : IValueConverter
    {
        public Style WallStyle { get; set; }
        public Style TabletStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is HostType type)
            {
                switch (type)
                {
                    case HostType.TouchscreenWall:
                    case HostType.SimpleWall:
                        return WallStyle;
                    case HostType.Tablet:
                        return TabletStyle;
                    default:
                        return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
