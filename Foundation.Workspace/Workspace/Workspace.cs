using Foundation.Core;
using Foundation.Core.ControlData;
using Foundation.Core.ViewModels;
using Foundation.Workspace.Menu;
using Foundation.Workspace.Windowing;
using GhostCore;
using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using UIMenu = Foundation.Workspace.Menu.Menu;

namespace Foundation.Workspace
{
    [TemplatePart(Name = "PART_BackgroundHost", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_MenuHost", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_Menu", Type = typeof(UIMenu))]
    public sealed class Workspace : Control
    {
        #region Dependency Properties

        public static readonly DependencyProperty WorkspaceBackgroundProperty =
            DependencyProperty.Register("WorkspaceBackground", typeof(WorkspaceBackgroundViewModel), typeof(Workspace), new PropertyMetadata(null));

        public static readonly DependencyProperty AppDataProperty =
           DependencyProperty.Register("AppData", typeof(SessionViewModel), typeof(Workspace), new PropertyMetadata(null));

        public static readonly DependencyProperty WindowDataSourceProperty =
            DependencyProperty.Register("WindowDataSource", typeof(ObservableCollection<ContentItemViewModel>), typeof(Workspace), new PropertyMetadata(new ObservableCollection<ContentItemViewModel>()));

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(Workspace), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsControlStyleProperty =
            DependencyProperty.Register("ItemsControlStyle", typeof(Style), typeof(Workspace), new PropertyMetadata(null));

        #endregion

        #region Events

        public event EventHandler TemplateApplied;
        private void OnTemplateApplied()
        {
            if (TemplateApplied == null)
                return;

            TemplateApplied(this, EventArgs.Empty);
        }

        #endregion

        #region Fields

        private readonly WindowManagerBase _winMan;
        private readonly WorkspaceBackgroundManager _backMan;
        private readonly WorkspaceScreensaverManager _screenSaverMan;

        #endregion

        #region Properties
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }
        public WorkspaceBackgroundViewModel WorkspaceBackground
        {
            get { return (WorkspaceBackgroundViewModel)GetValue(WorkspaceBackgroundProperty); }
            set { SetValue(WorkspaceBackgroundProperty, value); }
        }

        public SessionViewModel AppData
        {
            get { return (SessionViewModel)GetValue(AppDataProperty); }
            set { SetValue(AppDataProperty, value); }
        }

        public ObservableCollection<ContentItemViewModel> WindowDataSource
        {
            get { return (ObservableCollection<ContentItemViewModel>)GetValue(WindowDataSourceProperty); }
            set { SetValue(WindowDataSourceProperty, value); }
        }

        public Style ItemsControlStyle
        {
            get { return (Style)GetValue(ItemsControlStyleProperty); }
            set { SetValue(ItemsControlStyleProperty, value); }
        }

        #endregion

        #region Template Parts

        public Panel PART_BackgroundHost { get; private set; }
        public Panel PART_ScreensaverHost { get; private set; }
        public Panel PART_MenuHost { get; private set; }
        public ListView PART_ItemsHost { get; private set; }
        public UIMenu PART_Menu { get; private set; }

        #endregion

        #region Initialization

        public Workspace()
        {
            DefaultStyleKey = typeof(Workspace);

            if (DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled)
                return;

            var rootConfig = ServiceLocator.Instance.Resolve<RootConfigurationViewModel>();
            if (rootConfig.HostType == HostType.Tablet)
            {
                _winMan = new TabletWindowManager(this);
                _backMan = new WorkspaceBackgroundManager(this) { EnableBlurring = false };
            }
            else
            {
                _winMan = new WindowManager(this);
                _backMan = new WorkspaceBackgroundManager(this) { EnableBlurring = true };
            }

            if (rootConfig.HostType != HostType.Tablet)
                _screenSaverMan = new WorkspaceScreensaverManager(this);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_BackgroundHost = GetTemplateChild(nameof(PART_BackgroundHost)) as Panel;
            PART_ItemsHost = GetTemplateChild(nameof(PART_ItemsHost)) as ListView;
            PART_MenuHost = GetTemplateChild(nameof(PART_MenuHost)) as Panel;
            PART_Menu = GetTemplateChild(nameof(PART_Menu)) as UIMenu;
            PART_ScreensaverHost = GetTemplateChild(nameof(PART_ScreensaverHost)) as Panel;

            PART_BackgroundHost.IsHoldingEnabled = true;
            PART_BackgroundHost.IsDoubleTapEnabled = true;
            PART_BackgroundHost.IsRightTapEnabled = true;
            PART_BackgroundHost.IsTapEnabled = true;

            AttachHandlers();

            OnTemplateApplied();
        }

        private void AttachHandlers()
        {
            Unloaded += Workspace_Unloaded;
            PART_BackgroundHost.Tapped += PART_BackgroundHost_Tapped;
            PART_BackgroundHost.Holding += PART_BackgroundHost_Holding;
            PART_BackgroundHost.DoubleTapped += PART_BackgroundHost_DoubleTapped;
            PART_BackgroundHost.RightTapped += PART_BackgroundHost_RightTapped;

            PART_Menu.ManipulationMode = ManipulationModes.All;
            PART_Menu.ManipulationDelta += PART_Menu_ManipulationDelta;
        }

        #endregion

        #region Background Handlers

        private void PART_BackgroundHost_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var pos = e.GetPosition(PART_BackgroundHost);

            SpawnMenu(pos);
        }
        private void PART_BackgroundHost_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var pos = e.GetPosition(PART_BackgroundHost);

            SpawnMenu(pos);
        }
        private void PART_BackgroundHost_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }
        private void PART_BackgroundHost_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
        }


        #endregion

        #region Menu Handlers & Logic

        private void PART_Menu_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var curX = Canvas.GetLeft(PART_Menu);
            var curY = Canvas.GetTop(PART_Menu);

            var target = new Point(curX + e.Delta.Translation.X, curY + e.Delta.Translation.Y);
            PART_Menu.MoveTo(target, PART_MenuHost, useAnimation: false);
        }

        private void SpawnMenu(Point pos)
        {
            if (!PART_Menu.IsActive)
            {
                PART_Menu.IsActive = true;
                PART_Menu.MoveTo(pos, PART_MenuHost, useAnimation: false);
            }
            else
            {
                PART_Menu.MoveTo(pos, PART_MenuHost);
            }
        }

        #endregion

        #region Cleanup

        private void DetatchHandlers()
        {
            Unloaded -= Workspace_Unloaded;
            PART_BackgroundHost.Tapped -= PART_BackgroundHost_Tapped;
            PART_BackgroundHost.Holding -= PART_BackgroundHost_Holding;
            PART_BackgroundHost.DoubleTapped -= PART_BackgroundHost_DoubleTapped;
            PART_BackgroundHost.RightTapped -= PART_BackgroundHost_RightTapped;

            PART_Menu.ManipulationDelta -= PART_Menu_ManipulationDelta;
        }

        private void Workspace_Unloaded(object sender, RoutedEventArgs e)
        {
            DetatchHandlers();
        }

        #endregion
    }
}
