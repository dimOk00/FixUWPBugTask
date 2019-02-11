using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Foundation.Clients.MableArch.Controls.Menu
{
    public sealed partial class AnimatedLogo : UserControl
    {
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public Visibility UnderlineVisibility
        {
            get { return (Visibility)GetValue(UnderlineVisibilityProperty); }
            set { SetValue(UnderlineVisibilityProperty, value); }
        }

        public static readonly DependencyProperty UnderlineVisibilityProperty =
            DependencyProperty.Register("UnderlineVisibility", typeof(Visibility), typeof(AnimatedLogo), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(AnimatedLogo), new PropertyMetadata(false, OnIsActiveChanged));

        private static async void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as AnimatedLogo;
            var val = (bool)e.NewValue;

            if (val)
            {
                var onAnim = that.Resources["AnimateOn"] as Storyboard;
                await Task.Delay(1200);
                onAnim.Begin();
            }
            else
            {
                that.image.Opacity = 0;
                that.image1.Opacity = 0;
                that.image2.Opacity = 0;
                that.image3.Opacity = 0;
                that.image4.Opacity = 0;
                that.image5.Opacity = 0;
                that.image6.Opacity = 0;
                that.image7.Opacity = 0;
                that.lbl1.Opacity = 0;
                that.rect.Opacity = 0;
            }

        }

        public AnimatedLogo()
        {
            InitializeComponent();
        }
    }
}
