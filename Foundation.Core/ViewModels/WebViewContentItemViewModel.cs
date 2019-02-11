using Foundation.Core.Data;
using GhostCore.UWP.AutoFormGeneration;
using System;

namespace Foundation.Core.ViewModels
{
    public class WebViewContentItemViewModel : ContentItemViewModel
    {
        public Uri URL
        {
            get { return ModelAs<WebContentItem>().URL; }
            set { ModelAs<WebContentItem>().URL = value; OnPropertyChanged(nameof(URL)); }
        }

        public WebViewContentItemViewModel(ContentItem model)
           : base(model)
        {
        }
    }
}
