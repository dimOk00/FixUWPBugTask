using Foundation.Controls.Imaging;
using Foundation.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using GhostCore.UWP.Extensions;
using System.Threading.Tasks;
using GhostCore.MVVM.Messaging;
using Foundation.Networking;
using Foundation.Core.ViewModels;
using Foundation.Shared.Net.Events;
using Foundation.Shared.Net;
using System.Collections.ObjectModel;
using GhostCore;

namespace Foundation.Workspace.Menu
{
    [TemplatePart(Name = "PART_Logo", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_MenuItems", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_CloseAll", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Back", Type = typeof(Button))]
    [TemplateVisualState(GroupName = "NormalStates", Name = "MainItems")]
    [TemplateVisualState(GroupName = "NormalStates", Name = "CloseAll")]
    public class Menu : Control
    {
        public event EventHandler MenuClosed;

        protected Storyboard _xAnimStoryboard;
        protected Storyboard _yAnimStoryboard;
        protected DoubleAnimation _xAnim;
        protected DoubleAnimation _yAnim;
        protected string _currentState;


        public FrameworkElement PART_Logo { get; private set; }
        public ListView PART_MenuItems { get; private set; }
        public Button PART_CloseAll { get; private set; }
        public Button PART_Back { get; private set; }
        public EventBus MenuSelectedItemBus { get; set; }
        public EventBus RemoteBus { get; set; }

        public object MenuItemsSource
        {
            get { return (object)GetValue(MenuItemsSourceProperty); }
            set { SetValue(MenuItemsSourceProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public MenuSettingsViewModel MenuSettings
        {
            get { return (MenuSettingsViewModel)GetValue(MenuSettingsProperty); }
            set { SetValue(MenuSettingsProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public ClientInfo SelectedTarget
        {
            get { return (ClientInfo)GetValue(SelectedTargetProperty); }
            set { SetValue(SelectedTargetProperty, value); }
        }

        public ObservableCollection<ClientInfo> RemoteTargetSource
        {
            get { return (ObservableCollection<ClientInfo>)GetValue(RemoteTargetSourceProperty); }
            internal set { SetValue(RemoteTargetSourceProperty, value); }
        }

        public bool ShowRemoteTargetSelect
        {
            get { return (bool)GetValue(ShowRemoteTargetSelectProperty); }
            set { SetValue(ShowRemoteTargetSelectProperty, value); }
        }

        public static readonly DependencyProperty ShowRemoteTargetSelectProperty =
            DependencyProperty.Register("ShowRemoteTargetSelect", typeof(bool), typeof(Menu), new PropertyMetadata(false));

        public static readonly DependencyProperty RemoteTargetSourceProperty =
            DependencyProperty.Register("RemoteTargetSource", typeof(ObservableCollection<ClientInfo>), typeof(Menu), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedTargetProperty =
            DependencyProperty.Register("SelectedTarget", typeof(ClientInfo), typeof(Menu), new PropertyMetadata(null, OnSelectedTargetChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Menu), new PropertyMetadata(null, OnSelectedItemChanged));
        public static readonly DependencyProperty MenuSettingsProperty =
            DependencyProperty.Register("MenuSettings", typeof(MenuSettingsViewModel), typeof(Menu), new PropertyMetadata(null));
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(Menu), new PropertyMetadata(false));
        public static readonly DependencyProperty MenuItemsSourceProperty =
            DependencyProperty.Register("MenuItemsSource", typeof(object), typeof(Menu), new PropertyMetadata(null));

        private static void OnSelectedTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as Menu;
            that.ShowRemoteTargetSelect = false;
        }


        public Menu()
        {
            var config = ServiceLocator.Instance.Resolve<RemoteRelayConfiguration>();
            var remoteHandler = config.Handler;

            //RemoteTargetSource = remoteHandler.OnlineClients;

            RemoteTargetSource = new ObservableCollection<ClientInfo>()
            {
                new ClientInfo() { HostType = HostType.Tablet, Label = "Offline Mode" },
                new ClientInfo() { HostType = HostType.SimpleWall, Label = "LED Wall 1" },
                new ClientInfo() { HostType = HostType.SimpleWall, Label = "LED Wall 2" },
                new ClientInfo() { HostType = HostType.TouchscreenWall, Label = "84\" Wall" },
            };

            SelectedTarget = RemoteTargetSource[0];

            DefaultStyleKey = typeof(Menu);
            MenuSelectedItemBus = EventBusManager.Instance.GetOrCreateBus(nameof(ContentItemViewModel.MenuSelectedItemBus));
            RemoteBus = EventBusManager.Instance.GetRemoteEventBus();
            InitializeStoryboards();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Logo = GetTemplateChild(nameof(PART_Logo)) as FrameworkElement;
            PART_MenuItems = GetTemplateChild(nameof(PART_MenuItems)) as ListView;
            PART_CloseAll = GetTemplateChild(nameof(PART_CloseAll)) as Button;
            PART_Back = GetTemplateChild(nameof(PART_Back)) as Button;

            VisualStateManager.GoToState(this, "MainItems", useTransitions: false);
            _currentState = "MainItems";

            PART_CloseAll.Click += PART_CloseAll_Click;
            PART_Logo.DoubleTapped += PART_Logo_DoubleTapped;
            PART_Back.Click += PART_Back_Click;

            PART_MenuItems.SelectionChanged += PART_MenuItems_SelectionChanged;
        }
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as Menu;

            that.MenuSelectedItemBus.Publish(e.NewValue, that);
        }


        private void PART_MenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            MenuSelectedItemBus.Publish(e.AddedItems[0], this);
        }

        private void InitializeStoryboards()
        {
            var animDuration = new Duration(TimeSpan.FromMilliseconds(300));
            var easing = new CubicEase();

            var sbX = new Storyboard();
            var sbY = new Storyboard();

            Storyboard.SetTarget(sbX, this);
            Storyboard.SetTargetProperty(sbX, "(Canvas.Left)");

            Storyboard.SetTarget(sbY, this);
            Storyboard.SetTargetProperty(sbY, "(Canvas.Top)");

            _xAnim = new DoubleAnimation()
            {
                To = 0,
                Duration = animDuration,
                EasingFunction = easing,
                EnableDependentAnimation = true,
            };

            _yAnim = new DoubleAnimation()
            {
                To = 0,
                Duration = animDuration,
                EasingFunction = easing,
                EnableDependentAnimation = true,
            };

            sbX.Children.Add(_xAnim);
            sbY.Children.Add(_yAnim);

            _xAnimStoryboard = sbX;
            _yAnimStoryboard = sbY;
        }

        internal void MoveTo(Point p, Panel container, bool useAnimation = true)
        {
            p = AdjustPosition(p, container);

            if (useAnimation)
                AnimateTo(p);
            else
                TeleportTo(p);

            RemoteBus.Publish(new RemoteEvent(RemoteEventType.MoveMenuTo, new MoveToPayload() { Animate = useAnimation, X = p.X, Y = p.Y }), this);
        }

        private Point AdjustPosition(Point p, Panel container)
        {
            if (p.X < 0)
            {
                p.X = 0;
            }

            if (p.Y < 0)
            {
                p.Y = 0;
            }

            if (p.X > container.ActualWidth - ActualWidth)
            {
                p.X = container.ActualWidth - ActualWidth;
            }

            if (p.Y > container.ActualHeight - ActualHeight)
            {
                p.Y = container.ActualHeight - ActualHeight;
            }

            return p;
        }

        private void TeleportTo(Point p)
        {
            Canvas.SetLeft(this, p.X);
            Canvas.SetTop(this, p.Y);
        }

        private void AnimateTo(Point p)
        {
            _xAnim.To = p.X;
            _yAnim.To = p.Y;

            _xAnimStoryboard.Begin();
            _yAnimStoryboard.Begin();
        }

        private void PART_CloseAll_Click(object sender, RoutedEventArgs e)
        {
            OnMenuClosed();
            IsActive = false;
            VisualStateManager.GoToState(this, "MainItems", useTransitions: false);
            _currentState = "MainItems";
            PART_MenuItems.SelectedItem = null;
        }
        private void PART_Back_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "MainItems", useTransitions: false);
            _currentState = "MainItems";
        }
        private void PART_Logo_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (!IsActive)
                return;

            if (_currentState == "CloseAll")
            {
                VisualStateManager.GoToState(this, "MainItems", useTransitions: false);
                _currentState = "MainItems";
                return;
            }

            if (_currentState == "MainItems")
            {
                VisualStateManager.GoToState(this, "CloseAll", useTransitions: false);
                _currentState = "CloseAll";
                return;
            }
        }



        private void OnMenuClosed()
        {
            if (MenuClosed == null)
                return;

            MenuClosed(this, EventArgs.Empty);
        }

    }
}
