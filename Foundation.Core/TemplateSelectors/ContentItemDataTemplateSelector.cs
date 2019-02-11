using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Core.TemplateSelectors
{
    public class ContentItemDataTemplateSelector : DataTemplateSelector
    {
        protected Dictionary<ContentItemType, DataTemplate> _templates;
        protected bool _isInit;

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate TemplateNotSetTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }
        public DataTemplate PDFTemplate { get; set; }
        public DataTemplate MultimediaControlTemplate { get; set; }
        public DataTemplate GalleryControlTemplate { get; set; }
        public DataTemplate HostedLayoutTemplate { get; set; }
        public DataTemplate WebViewTemplate { get; set; }
      

        private void Initialize()
        {
            _templates = new Dictionary<ContentItemType, DataTemplate>
            {
                { ContentItemType.Image, ImageTemplate },
                { ContentItemType.PDF, PDFTemplate },
                { ContentItemType.Video, VideoTemplate },
                { ContentItemType.MultimediaControl, MultimediaControlTemplate },
                { ContentItemType.Gallery, GalleryControlTemplate },
                { ContentItemType.HostedLayout, HostedLayoutTemplate },
                { ContentItemType.WebView, WebViewTemplate },
            };

            _isInit = true;
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!_isInit)
                Initialize();

            if (item is ContentItemViewModel vm)
            {
                if (vm.Type == ContentItemType.ProjectSpecific)
                    return null;

                if (!_templates.ContainsKey(vm.Type))
                    return DefaultTemplate;

                var template = _templates[vm.Type];
                if (template == null)
                    return TemplateNotSetTemplate;

                return template;
            }

            return DefaultTemplate;
        }
    }
}
