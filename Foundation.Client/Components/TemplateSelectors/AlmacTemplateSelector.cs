using Foundation.Clients.MableArch.Controls.ApartmentFinder;
using Foundation.Clients.MableArch.Controls.Dial;
using Foundation.Core.Data;
using Foundation.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Foundation.Clients.MableArch.Components.TemplateSelectors
{
    public class AlmacTemplateSelector : FoundationTemplateSelector
    {
        public DataTemplate DialTemplate { get; set; }
        public DataTemplate AptFinderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var template = base.SelectTemplateCore(item, container);
            if (template != null)
                return template;

            if (item is DialContentItemViewModel)
            {
                return DialTemplate;
            }

            if (item is ApartmentFinderViewModel)
            {
                return AptFinderTemplate;
            }

            return DefaultTemplate;
        }
    }
}
