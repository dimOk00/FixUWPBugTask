using Foundation.Clients.MableArch.Components.ViewModels;
using Foundation.Clients.MableArch.Controls;
using Foundation.Core;
using Foundation.Core.ControlData;
using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using Foundation.Networking;
using GhostCore;
using GhostCore.MVVM.Dynamic;
using GhostCore.UWP.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Foundation.Clients.MableArch
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var eventSerializer = new DefaultEventSerializer();
            ServiceLocator.Instance.Register(new RemoteRelayConfiguration()
            {
                EventSerializer = eventSerializer,
                Handler = new RelayHubCallbackHandler(eventSerializer),
            });
            ServiceLocator.Instance.Register<IViewModelFactory<ContentItemViewModel, ContentItem>>(new CustomContentItemViewModelFactoryFactory());

#if !DEBUG
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
#endif
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(LoadingPage), e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
