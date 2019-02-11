using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public sealed class DialMenuSectionControl : ItemsControl
    {
        private const double TransitionDuration = 500;

        #region Section DP
        public SectionViewModel Section
        {
            get { return (SectionViewModel)GetValue(SectionProperty); }
            set { SetValue(SectionProperty, value); }
        }

        public static readonly DependencyProperty SectionProperty =
            DependencyProperty.Register("Section", typeof(SectionViewModel), typeof(DialMenuSectionControl), new PropertyMetadata(null, OnSectionChanged));

        private static void OnSectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = d as DialMenuSectionControl;
            thisControl.Initialise();
        }
        #endregion

        public DialMenuSectionControl()
        {
            Loaded += DialMenuSectionControl_Loaded;
            this.DefaultStyleKey = typeof(DialMenuSectionControl);
        }

        private void DialMenuSectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Section?.IsCurrentSection == true)
            {
                Show(1000);
            }
        }

        public void Show(double initialDelay = 2000)
        {
            foreach (var item in Section.MenuItems.Select((value, i) => new { i, value }))
            {
                var container = ContainerFromItem(item.value) as ContentPresenter;
                if (container != null)
                {
                    var delay = initialDelay + (item.i * 300);
                    if (item.value.HasSubMenu)
                    {
                        var menuItemControl = container.FindDescendant<MenuItemWithSubMenuUserControl>();
                        menuItemControl.ShowBar(delay + 2000);
                    }

                    container.Offset(300, duration:0).Fade(0, duration:0).Then()
                        .Offset(0, duration: TransitionDuration).Fade(1, duration: TransitionDuration).SetDelay(delay)
                        .Start();
                }
            }
        }

        public void Hide()
        {
            foreach (var item in Section.MenuItems.Select((value, i) => new { i, value }))
            {
                var container = ContainerFromItem(item.value) as ContentPresenter;
                if (container != null)
                {
                    var delay = (item.i * 250);
                    container.Fade(0, duration: TransitionDuration).SetDelay(delay).Start();
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var viewModel = item as MenuItemViewModel;
            var margin = new Thickness(viewModel.PosX, viewModel.PosY, 0, 0);

            var container = element as FrameworkElement;
            container.Margin = margin;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Initialise();
        }

        private void Initialise()
        {
            this.ItemsSource = Section?.MenuItems;
        }
    }
}
