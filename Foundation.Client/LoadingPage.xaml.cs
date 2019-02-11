using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Foundation.Clients.MableArch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        private void LoadingControl_LoadFinished(object sender, EventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void LoadingControl_LoadFailed(object sender, Exception ex)
        {
            pnlErrorGrid.Visibility = Visibility.Visible;
            LoadingControl.Visibility = Visibility.Collapsed;

            lblExceptionText.Text = ex.ToString();
        }

        private async void LoadingControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadingControl.LoadAppData();
        }
    }
}
