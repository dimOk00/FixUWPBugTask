using Foundation.Clients.MableArch.Controls.ApartmentFinder;
using Foundation.Clients.MableArch.Controls.Dial;
using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using System;

namespace Foundation.Clients.MableArch.Controls
{
    public class CustomContentItemViewModelFactoryFactory : DefaultContentItemViewModelFactory
    {
        public override ContentItemViewModel GetProjectSpecific(ContentItem item)
        {
            if (item is DialContentItem dial)
            {
                return new DialContentItemViewModel(dial);
            }

            if (item is ApartmentFinderItem apt)
            {
                return new ApartmentFinderViewModel(apt);
            }

            throw new Exception("Not all content item types were mapped");
        }
    }
}
