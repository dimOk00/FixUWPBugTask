using Foundation.Core.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Foundation.Clients.MableArch.Controls.Menu
{
    public class MenuSeparatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (value is IEnumerable<ContentItemViewModel> qw)
            {
                var count = qw.Count(x => !x.IsHidden);
                return count  == 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            if (value is IEnumerable e)
                return e.Cast<object>().Count() == 0 ? Visibility.Collapsed : Visibility.Visible;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
