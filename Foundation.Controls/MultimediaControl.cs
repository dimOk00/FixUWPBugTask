using Foundation.Controls.Abstract;
using Foundation.Controls.Imaging;
using Foundation.Controls.Managers;
using Foundation.Core.Data;
using Foundation.Core.Data.Abstract;
using Foundation.Core.Events.Windows;
using Foundation.Core.ViewModels;
using Foundation.Networking;
using Foundation.Shared.Net.Events;
using GhostCore;
using GhostCore.MVVM;
using GhostCore.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Foundation.Controls
{
    public sealed class MultimediaControl : FlipView
    {
        private ObservableCollection<ContentItemViewModel> _source;
        private EventBus _windowOperationsBus;
        private EventBus _remoteMultimediaOpsBus;
        private EventBus _remoteEventBus;
        private bool _parentWindowLocked;
        private AutoInferManager _autoInferManager;
        private DispatcherTimer _slideshowTimer;
        private bool _allowSelectionChanged = true;

        public MultimediaContentItemViewModel ViewModel
        {
            get { return (MultimediaContentItemViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public bool AreSizingEventsEnabled
        {
            get { return (bool)GetValue(AreSizingEventsEnabledProperty); }
            set { SetValue(AreSizingEventsEnabledProperty, value); }
        }

        public bool IsSlideshow
        {
            get { return (bool)GetValue(IsSlideshowProperty); }
            set { SetValue(IsSlideshowProperty, value); }
        }

        public TimeSpan ReleaseTime
        {
            get { return (TimeSpan)GetValue(ReleaseTimeProperty); }
            set { SetValue(ReleaseTimeProperty, value); }
        }

        public TimeSpan SlideInterval
        {
            get { return (TimeSpan)GetValue(SlideIntervalProperty); }
            set { SetValue(SlideIntervalProperty, value); }
        }

        public static readonly DependencyProperty ReleaseTimeProperty =
            DependencyProperty.Register("ReleaseTime", typeof(TimeSpan), typeof(MultimediaControl), new PropertyMetadata(TimeSpan.FromSeconds(0.5)));

        public static readonly DependencyProperty SlideIntervalProperty =
            DependencyProperty.Register("SlideInterval", typeof(TimeSpan), typeof(MultimediaControl), new PropertyMetadata(TimeSpan.FromSeconds(3), OnSlideIntervalChanged));

        public static readonly DependencyProperty IsSlideshowProperty =
            DependencyProperty.Register("IsSlideshow", typeof(bool), typeof(MultimediaControl), new PropertyMetadata(false, OnIsSlideshowChanged));

        public static readonly DependencyProperty AreSizingEventsEnabledProperty =
            DependencyProperty.Register("AreSizingEventsEnabled", typeof(bool), typeof(MultimediaControl), new PropertyMetadata(true));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MultimediaContentItemViewModel), typeof(MultimediaControl), new PropertyMetadata(null, OnViewModelChanged));

        public MultimediaControl()
        {
            _source = new ObservableCollection<ContentItemViewModel>();
            _autoInferManager = new AutoInferManager(_source);

            _slideshowTimer = new DispatcherTimer();
            _slideshowTimer.Tick += slideshowTimer_Tick;

            PointerPressed += MultimediaControl_PointerPressed;
            AddHandler(PointerReleasedEvent, new PointerEventHandler(MultimediaControl_PointerReleased), true);

            ItemsSource = _source;

            _windowOperationsBus = EventBusManager.Instance.GetOrCreateBus("WindowOperationBus");
            _remoteMultimediaOpsBus = EventBusManager.Instance.GetOrCreateBus("RemoteMultimediaOpsBus");

            _remoteEventBus = EventBusManager.Instance.GetRemoteEventBus();

            _windowOperationsBus.EventBroadcasted += WindowOperationsBus_EventBroadcasted;
            _remoteMultimediaOpsBus.EventBroadcasted += RemoteMultimediaOpsBus_EventBroadcasted;

            SelectionChanged += MultimediaControl_SelectionChanged;

            Loaded += MultimediaControl_Loaded;
        }

        private void MultimediaControl_Loaded(object sender, RoutedEventArgs e)
        {
            var rootConfig = ServiceLocator.Instance.Resolve<RootConfigurationViewModel>();
            if (rootConfig != null && rootConfig.HostType == Core.HostType.Tablet)
                IsHitTestVisible = true;
        }

        private void MultimediaControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (IsSlideshow)
                _slideshowTimer.Start();
        }

        private void MultimediaControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (IsSlideshow)
                _slideshowTimer.Stop();
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as MultimediaControl;
            that.OnViewModelChanged(e.NewValue as MultimediaContentItemViewModel);
        }

        private static void OnIsSlideshowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as MultimediaControl;
            if (e.NewValue is bool b)
            {
                that._slideshowTimer.Interval = that.SlideInterval;
                if (b)
                    that._slideshowTimer.Start();
            }
        }
        private static void OnSlideIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as MultimediaControl;
            if (that._slideshowTimer.IsEnabled)
                that._slideshowTimer.Stop();
            that._slideshowTimer.Interval = (TimeSpan)e.NewValue;

            if (that.IsSlideshow)
                that._slideshowTimer.Start();
        }


        private async void OnViewModelChanged(MultimediaContentItemViewModel vm)
        {
            if (vm == null)
            {
                _source.Clear();
                return;
            }

            IsSlideshow = vm.IsSlideshow;
            SlideInterval = vm.SlideInterval;

            if (vm.AutoInferFromPath)
            {
                await _autoInferManager.AutoInferFromPath(vm, vm.Path.LocalPath);
                return;
            }

            foreach (var x in vm.Children)
            {
                _source.Add(x);
            }
            SelectedItem = vm.SelectedMultimediaItem;
        }

        private async void MultimediaControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!AreSizingEventsEnabled || !_allowSelectionChanged)
                return;

            //because we can only have 1 item selected at a time, it's ok to use AddedItems[0] and RemovedItems[0]

            double aw = 0, ah = 0;
            bool allowAnim = true;

            var itm = e.AddedItems[0];
            var container = ContainerFromItem(itm);
            var fvi = container as FlipViewItem;
            var root = fvi?.ContentTemplateRoot;

            if (root == null)
            {
                if (itm is IMultimediaItem i)
                {
                    var size = await i.GetSize();
                    var ar = size.Width / size.Height;
                    ah = ActualHeight;
                    aw = ah * ar;

                    allowAnim = false;

                    goto SPARK;

                }
                else
                    return;
            }

            if (e.RemovedItems.Count == 1) //Initial Selection Check
            {
                var removed = e.RemovedItems[0];
                var removedCont = ContainerFromItem(removed) as FlipViewItem;
                var removedRoot = removedCont?.ContentTemplateRoot;
                if (removedRoot is IMultimediaControl mc)
                {
                    mc.Deactivate();
                }
            }
            else
            {
                allowAnim = false;
            }

            if (root is IMultimediaControl mmc)
            {
                mmc.Activate();
                var size = await mmc.GetExpectedSize();
                aw = size.Width;
                ah = size.Height;
            }


            if (aw == 0 || ah == 0)
                return;

            SPARK:
            var wndOp = new WindowOperationEventData()
            {
                DataObject = new ContentSizeChangedData() { Size = new Size(aw, ah), AllowAnimation = allowAnim },
                Operation = WindowOperation.ContentChanged
            };
            _windowOperationsBus.Publish(wndOp, ViewModel);

            _remoteEventBus.Publish(new RemoteEvent(RemoteEventType.MultimediaControlSelectionChanged,
                new SelectionChangedPayload()
                {
                    SelectedIndex = SelectedIndex,
                    Context = ViewModel.Model
                }), this);
        }
        private async void RemoteMultimediaOpsBus_EventBroadcasted(BusEvent e)
        {
            var dsp = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            if (!dsp.HasThreadAccess)
            {
                await dsp.RunAsync(CoreDispatcherPriority.Normal, () => RemoteMultimediaOpsBus_EventBroadcasted(e));
                return;
            }

            var remoteEvt = e.DataAs<RemoteEvent>();
            if (remoteEvt.Type == RemoteEventType.MultimediaControlSelectionChanged)
            {
                var payload = remoteEvt.Payload as SelectionChangedPayload;
                var vm = ViewModelMapper.GetViewModel(payload.Context);

                if (vm == ViewModel)
                {
                    _allowSelectionChanged = false;
                    SelectedIndex = payload.SelectedIndex;
                    _allowSelectionChanged = true;
                }
            }
        }

        private void WindowOperationsBus_EventBroadcasted(BusEvent e)
        {
            if (!AreSizingEventsEnabled)
                return;

            /* For future implementation:
             * Create another EventBus-like called Channel
             * This object will basically emit to objects that share the same context
             * Example:
             * Window creates a channel for it's vm (let's say a MultimediaVM)
             * the content (when it's created) gets the channel for that VM and all events are only between the window and content
             * 
             * Shared content might be interesting for network situations
            */

            var winOpData = e.DataAs<WindowOperationEventData>();
            if (winOpData.DataObject == ViewModel)
            {
                if (winOpData.Operation == WindowOperation.Lock)
                    _parentWindowLocked = true;

                if (winOpData.Operation == WindowOperation.Unlock)
                    _parentWindowLocked = false;

                if (winOpData.Operation == WindowOperation.Lock ||
                    winOpData.Operation == WindowOperation.Fullscreen)
                    IsHitTestVisible = true;

                if (winOpData.Operation == WindowOperation.Unlock ||
                    winOpData.Operation == WindowOperation.Restore)
                    IsHitTestVisible = false;

                if (winOpData.Operation == WindowOperation.Restore && _parentWindowLocked)
                    IsHitTestVisible = true;
            }
        }

        private void slideshowTimer_Tick(object sender, object e)
        {
            if (SelectedIndex == -1 || Items == null || Items.Count == 0)
                return;

            SelectedIndex = (SelectedIndex + 1) % Items.Count;
        }

    }

    public class ContentSizeChangedData
    {
        public Size Size { get; set; }
        public bool AllowAnimation { get; set; }
    }
}
