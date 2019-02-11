using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.UI.Xaml.Data;

namespace Foundation.Workspace.Converters
{
    public class UriToIMediaPlaybackSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Uri uri)
            {
                return MediaSource.CreateFromUri(uri);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is MediaSource ms)
            {
                return ms.Uri;
            }

            return null;
        }
    }
}
