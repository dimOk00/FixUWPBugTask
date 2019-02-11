using Foundation.Core.Data;
using GhostCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Foundation.Core.Converters
{
    public class AppConfigurationUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string path = null;

            if (value is string str)
            {
                if (str.Contains("file://") || str.Contains("http://") || str.Contains("https://"))
                    return new Uri(str);

                path = str;
            }

            if (value is Uri uri)
            {
                path = uri.ToString();
            }

            var config = ServiceLocator.Instance.Resolve<GlobalSettings>();
            var actualPath = config.GetFullPath(path);

            return new Uri(actualPath);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
