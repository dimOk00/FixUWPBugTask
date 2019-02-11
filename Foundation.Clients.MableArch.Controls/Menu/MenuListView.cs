using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Foundation.Clients.MableArch.Controls.Menu
{
    public class MenuListView : ListView
    {
        public MenuListView()
        {
            IsItemClickEnabled = true;
            SelectionMode = ListViewSelectionMode.None;
            ItemClick += MenuListView_ItemClick;
        }

        private void MenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var contItm = (ListViewItem)ContainerFromItem(e.ClickedItem);

            if (SelectedItem == e.ClickedItem)
            {
                RevertOpacity((SelectedItem as ContentItemViewModel).Children);
                SelectedItem = null;
            }
            else
            {
                SelectedItem = e.ClickedItem;
                contItm.IsSelected = true;

                var source = (e.ClickedItem as ContentItemViewModel).Children;
                RevertOpacity(source);
                SetupAnimations(source);
            }
        }

        private async void SetupAnimations(IEnumerable<ContentItemViewModel> source)
        {
            await Task.Delay(500);

            foreach (var x in source)
            {
                x.ShouldAnimateOpacity = true;
                await Task.Run(async () =>
                {
                    var totalMs = 400d;
                    while (totalMs != 0 && x.ShouldAnimateOpacity)
                    {

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            x.Opacity = 1 - (totalMs / 400);
                        });

                        await Task.Delay(16);
                        totalMs -= 16;
                    }
                });

                await Task.Delay(250);
            }
        }

        private static void RevertOpacity(IEnumerable<ContentItemViewModel> source)
        {
            foreach (var x in source)
            {
                x.ShouldAnimateOpacity = false;
                x.Opacity = 0;
            }
        }

        //private void SetupAnimations(IEnumerable<ContentItemViewModel> source)
        //{
        //    var sb = new Storyboard();

        //    var delay = TimeSpan.FromMilliseconds(400);

        //    foreach (var x in source)
        //    {
        //        var opacityAnim = new DoubleAnimation
        //        {
        //            EasingFunction = new CubicEase(),
        //            Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromMilliseconds(1000)),
        //            BeginTime = delay,
        //            EnableDependentAnimation = true,
        //            From = 0,
        //            To = 1
        //        };

        //        Storyboard.SetTarget(opacityAnim, x);
        //        Storyboard.SetTargetProperty(opacityAnim, "Opacity");
        //        sb.Children.Add(opacityAnim);

        //        delay = delay.Add(TimeSpan.FromMilliseconds(250));
        //    }

        //    sb.Begin();
        //}
    }


    public class ListViewAnimator
    {

    }
}
