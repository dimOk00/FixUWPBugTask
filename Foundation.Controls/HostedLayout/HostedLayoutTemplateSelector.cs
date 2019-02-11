using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Controls.HostedLayout
{
    public class HostedLayoutTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmptyTemplate { get; set; } = new DataTemplate();
        public DataTemplate EmptyContentAreaTemplate { get; set; }
        public DataTemplate ButtonItemTemplate { get; set; }
        public DataTemplate ToggleItemTemplate { get; set; }
        public DataTemplateSelector ContentItemDataTemplateSelector { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ContentAreaViewModel cavm)
            {
                if (cavm.HasContent)
                {
                    return ContentItemDataTemplateSelector.SelectTemplate(cavm.Content, container);
                }
                else
                {
                    return EmptyContentAreaTemplate;
                }
            }

            if (item is ButtonItemViewModel)
            {
                return ButtonItemTemplate;
            }

            if (item is ToggleItemViewModel)
            {
                return ToggleItemTemplate;
            }

            if (item is InteractableItemViewModel)
            {
                return EmptyTemplate;
            }


            return null;
        }
    }

}
