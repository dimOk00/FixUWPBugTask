using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public sealed partial class MenuItemWithSubMenuUserControl : UserControl
    {
        public MenuItemWithSubMenuUserControl()
        {
            this.InitializeComponent();
        }

        public void ShowBar(double delayMilliseconds)
        {
            var storyboard = new Storyboard();
            var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            scaleXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = 0
            });
            scaleXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(delayMilliseconds)),
                Value = 0
            });
            scaleXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(delayMilliseconds+1000)),
                Value = 1
            });
            Storyboard.SetTarget(scaleXAnimation, MyRect);
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Begin();
        }
    }
}
