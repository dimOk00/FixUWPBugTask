using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Controls.TemplateSelectors
{
    public class MultimediaTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }
        public DataTemplate PDFTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var type = item.GetType();

            if (type == typeof(ImageContentItemViewModel))
                return ImageTemplate;

            if (type == typeof(VideoContentItemViewModel))
                return VideoTemplate;

            if (type == typeof(PDFContentItemViewModel))
                return PDFTemplate;

            return null;
        }
    }
}
