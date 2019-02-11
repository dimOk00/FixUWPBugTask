using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class MenuItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MenuItemDataTemplate { get; set; }
        public DataTemplate MenuItemWithSubMenuDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            DataTemplate dataTemplate;
            var menuItemViewModel = item as MenuItemViewModel;
            dataTemplate = menuItemViewModel?.SubMenuItems?.Any() ?? false
                ? MenuItemWithSubMenuDataTemplate
                : MenuItemDataTemplate;

            return dataTemplate;
        }
    }
}
