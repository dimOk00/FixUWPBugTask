using Foundation.Clients.MableArch.Components.ViewModels;
using Foundation.Core.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Core;

namespace Foundation.Clients.MableArch
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

        public MainPage()
        {
            DataContext = new MainPageViewModel();
            InitializeComponent();
        }
    }
}
