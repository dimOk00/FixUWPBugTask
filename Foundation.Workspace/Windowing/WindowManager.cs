using Foundation.Controls;
using Foundation.Core.Data;
using Foundation.Core.Events.Windows;
using Foundation.Core.ViewModels;
using Foundation.Networking;
using Foundation.Shared.Net.Events;
using GhostCore.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Window = Foundation.Workspace.Windowing.WorkspaceWindow;

namespace Foundation.Workspace.Windowing
{
    public class WindowManager : WindowManagerBase
    {
        #region Fields

        protected double _relativePosX;
        protected double _relativePosY;

        #endregion

        #region Init

        public WindowManager(Workspace workspace)
            : base(workspace)
        {
            _wndOpHandlers = new Dictionary<WindowOperation, Action<WindowOperationEventData>>
            {
                { WindowOperation.Opened, WndOpHandler_Opened },
                { WindowOperation.Close, WndOpHandler_Close },
                { WindowOperation.Lock, WndOpHandler_Lock },
                { WindowOperation.Unlock, WndOpHandler_Unlock},
                { WindowOperation.Fullscreen, WndOpHandler_Fullscreen },
                { WindowOperation.Restore, WndOpHandler_Restore },
                { WindowOperation.ContentChanged, WndOpHandler_ContentSizeChanged }
            };

            _workspace.Loaded += Workspace_Loaded;
        }


        private void Workspace_Loaded(object sender, RoutedEventArgs e)
        {
            _cascadeManager.SetMenu(_workspace.PART_Menu);
            _workspace.PART_Menu.MenuClosed += PART_Menu_MenuClosed;
        }

        #endregion

        #region Event Bus Handlers

        protected async override void SelectedMenuItemBus_EventBroadcasted(BusEvent e)
        {
            var dsp = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            if (!dsp.HasThreadAccess)
            {
                await dsp.RunAsync(CoreDispatcherPriority.Normal, () => SelectedMenuItemBus_EventBroadcasted(e));
                return;
            }

            if (e.DataObject == null)
            {
                _workspace.WindowDataSource.Clear();
                return;
            }

            if (e.DataObject is ContentItemViewModel cvi && cvi.Type != ContentItemType.MenuItem)
            {
                if (_rootConfigVm.HostType != Core.HostType.Tablet && cvi.Type == ContentItemType.Gallery && e.EventFilter == "remote")
                    return;


                if (_workspace.WindowDataSource.Contains(cvi))
                {
                    if (cvi.IsSingleton)
                    {
                        BringItemToFront(cvi);
                        return;
                    }
                    else
                    {
                        cvi = cvi.Clone();
                    }
                }


                _workspace.WindowDataSource.Add(cvi);

                if (cvi.PublishRemoteEvents && e.EventFilter == null)
                    _remoteBus.Publish(new RemoteEvent(RemoteEventType.MenuItemSelected, cvi.Model), this);

                var cont = (ListViewItem)_workspace.PART_ItemsHost.ContainerFromItem(cvi);

                if (cont == null)
                    return;

                if (!cvi.WindowSettings.ShrinkToContent)
                {
                    cont.Width = cvi.WindowSettings.DefaultWidth;
                    cont.Height = cvi.WindowSettings.DefaultHeight;
                }
                else
                {
                    cont.MaxWidth = cvi.WindowSettings.DefaultWidth;
                    cont.MaxHeight = cvi.WindowSettings.DefaultHeight;
                }
            }
        }

        protected async override void RemoteWindowOperationsBus_EventBroadcasted(BusEvent e)
        {
            var remoteOp = e.DataAs<RemoteWindowOperationEvent>();
            var vm = remoteOp.ViewModel;

            var wnd = _vmWindowMapping[(ContentItemViewModel)vm];

            var op = WindowOperation.Unlock;
            switch (remoteOp.RemoteEvent.Type)
            {
                case RemoteEventType.MenuClosed:
                case RemoteEventType.MoveMenuTo:
                case RemoteEventType.MenuItemSelected:
                    op = WindowOperation.Unknown;
                    break;
                case RemoteEventType.WindowClosed:
                    op = WindowOperation.Close;
                    break;
                case RemoteEventType.WindowLocked:
                    op = WindowOperation.Lock;
                    break;
                case RemoteEventType.WindowRestored:
                    op = WindowOperation.Restore;
                    break;
                case RemoteEventType.WindowUnlocked:
                    op = WindowOperation.Unlock;
                    break;
                default:
                    break;
            }

            var opdata = new WindowOperationEventData()
            {
                BusEvent = e,
                DataObject = vm,
                WindowObject = wnd,
                Operation = op
            };

            var dsp = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            if (!dsp.HasThreadAccess)
            {
                await dsp.RunAsync(CoreDispatcherPriority.Normal, () =>_wndOpHandlers[op](opdata));
                return;
            }

        }

        #endregion

        #region Window Operation Callbacks
        private void WndOpHandler_Opened(WindowOperationEventData obj)
        {
            var vm = obj.DataObject as ContentItemViewModel;

            if (obj.WindowObject is Window wnd)
            {
                wnd.ManipulationMode = ManipulationModes.All;
                wnd.Tapped += Wnd_Tapped;
                wnd.ManipulationStarted += Wnd_ManipulationStarted;
                wnd.ManipulationDelta += Wnd_ManipulationDelta;
                wnd.ManipulationCompleted += Wnd_ManipulationCompleted;

                var (x, y) = _cascadeManager.GetSpawnCoordinates();
                wnd.SetPosition(x, y);

                // HACK : this should be temporary since i don't know if it's ok to have multiple windows open under the same VM
                if (!_vmWindowMapping.ContainsKey(vm))
                    _vmWindowMapping.Add(vm, wnd);

                BringItemToFront(vm);
            }
        }
        private void WndOpHandler_Close(WindowOperationEventData data)
        {
            if (data.DataObject is ContentItemViewModel cvi && cvi.Type != ContentItemType.MenuItem)
            {
                _workspace.WindowDataSource.Remove(cvi);
                _vmWindowMapping.Remove(cvi);


                if (cvi.PublishRemoteEvents && data.BusEvent.EventFilter != null)
                    _remoteBus.Publish(new RemoteEvent(RemoteEventType.WindowClosed, cvi.Model), this);
            }
        }
        private void WndOpHandler_Fullscreen(WindowOperationEventData obj)
        {
            var wnd = obj.WindowObject as Window;
            var vm = wnd.DataContext as ContentItemViewModel;


            if (vm.PublishRemoteEvents && obj.BusEvent.EventFilter != null)
                _remoteBus.Publish(new RemoteEvent(RemoteEventType.WindowLocked, vm.Model), this);

            var cont = (ListViewItem)_workspace.PART_ItemsHost.ContainerFromItem(vm);

            if (cont == null)
                return;

            cont.MaxWidth = double.PositiveInfinity;
            cont.MaxHeight = double.PositiveInfinity;

            cont.Width = _workspace.PART_ItemsHost.ActualWidth;
            cont.Height = _workspace.PART_ItemsHost.ActualHeight;

            wnd.PrepareFullscreen();

        }
        private void WndOpHandler_Restore(WindowOperationEventData obj)
        {
            var wnd = obj.WindowObject as Window;
            var vm = wnd.DataContext as ContentItemViewModel;


            if (vm.PublishRemoteEvents && obj.BusEvent.EventFilter != null)
                _remoteBus.Publish(new RemoteEvent(RemoteEventType.WindowRestored, vm.Model), this);

            var cont = (ListViewItem)_workspace.PART_ItemsHost.ContainerFromItem(vm);

            if (cont == null)
                return;

            wnd.PrepareRestore();
            var (w, h) = wnd.GetRestoreData();
            cont.Width = w;
            cont.Height = h;
        }
        private void WndOpHandler_Lock(WindowOperationEventData obj)
        {
            var wnd = obj.WindowObject as Window;
            var vm = wnd.DataContext as ContentItemViewModel;

            if (vm.PublishRemoteEvents && obj.BusEvent.EventFilter != null)
                _remoteBus.Publish(new RemoteEvent(RemoteEventType.WindowLocked, vm.Model), this);
        }
        private void WndOpHandler_Unlock(WindowOperationEventData obj)
        {
            var wnd = obj.WindowObject as Window;
            var vm = wnd.DataContext as ContentItemViewModel;

            if (vm.PublishRemoteEvents && obj.BusEvent.EventFilter != null)
                _remoteBus.Publish(new RemoteEvent(RemoteEventType.WindowUnlocked, vm.Model), this);
        }
        private async void WndOpHandler_ContentSizeChanged(WindowOperationEventData obj)
        {
            var cvi = obj.BusEvent.OriginalSource as ContentItemViewModel;
            var e = (ContentSizeChangedData)obj.DataObject;

            var cont = (ListViewItem)_workspace.PART_ItemsHost.ContainerFromItem(cvi);

            if (cont == null)
                return;

            var wnd = _vmWindowMapping[cvi];
            var aspectRatio = e.Size.Width / e.Size.Height;
            double targetHeight = cont.ActualWidth / aspectRatio;
            double targetWidth = cont.ActualHeight * aspectRatio;

            if (wnd.IsFullscreen)
            {
                //wnd.ChangeRestoreData(cont.ActualWidth, cont.ActualHeight);
                return;
            }

            await Task.Delay(100);
            cont.MaxWidth = double.PositiveInfinity;

            if (e.AllowAnimation)
                MorphWidth(cont, targetWidth);
            else
                cont.Width = targetWidth;

            //if (cont.MaxHeight < targetHeight)
            //{
            //    cont.Width = targetWidth;
            //}
            //else
            //{
            //    if (cont.MaxWidth - 10 < targetWidth && targetWidth < cont.MaxWidth + 10)
            //    {
            //        cont.Width = cont.MaxWidth;
            //    }
            //    else
            //        cont.Height = targetHeight;
            //}
            //TODO size to content after it changes inside a+ resture

        }

        private void MorphWidth(ListViewItem cont, double targetWidth)
        {
            var sb = new Storyboard();
            var anim = new DoubleAnimation()
            {
                To = targetWidth,
                EnableDependentAnimation = true,
                Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                EasingFunction = new QuadraticEase()
            };

            Storyboard.SetTarget(anim, cont);
            Storyboard.SetTargetProperty(anim, "Width");

            sb.Children.Add(anim);

            sb.Begin();

            sb.Completed += (s, e) => cont.Width = targetWidth;
        }
        #endregion

        #region Chrome Handlers

        private void Wnd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EnsureTopmost(sender);
        }

        private void Wnd_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _relativePosX = e.Position.X;
            _relativePosY = e.Position.Y;

            EnsureTopmost(sender);
        }
        private void Wnd_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var wnd = sender as Window;

            if (wnd.IsLocked)
                return;

            var transform = wnd.GetTransform();
            var controlTransform = wnd.GetControlTransform();

            PerformTransation(wnd, e, transform);
            PerformScale(wnd, e, transform, controlTransform);
            //PerformRotation(wnd, e, transform);
        }
        private void Wnd_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
        }

        #endregion

        #region Transformations

        private void EnsureTopmost(object sender)
        {
            var wnd = sender as Window;
            var vm = wnd.DataContext as ContentItemViewModel;

            var cont = (ListViewItem)_workspace.PART_ItemsHost.ContainerFromItem(vm);

            if (cont == null)
                return;

            Canvas.SetZIndex(cont, ++_currentZIndex);
        }
        private void PerformRotation(Window wnd, ManipulationDeltaRoutedEventArgs e, CompositeTransform transform)
        {
            if (wnd.IsFullscreen)
                return;

            transform.Rotation += e.Delta.Rotation;
        }
        private void PerformScale(Window wnd, ManipulationDeltaRoutedEventArgs e, CompositeTransform transform, CompositeTransform compositeTransform)
        {
            if (wnd.IsFullscreen)
                return;

            if (e.Delta.Scale == 1)
                return;

            var minScale = _rootConfigVm.SelectedSession.Root.WindowSettings.MinWindowScale;
            var maxScale = _rootConfigVm.SelectedSession.Root.WindowSettings.MaxWindowScale;

            transform.ScaleX *= e.Delta.Scale;
            transform.ScaleY *= e.Delta.Scale;

            transform.TranslateX = (e.Delta.Scale * (transform.TranslateX - _relativePosX)) + _relativePosX;
            transform.TranslateY = (e.Delta.Scale * (transform.TranslateY - _relativePosY)) + _relativePosY;

            var scaledWidth = transform.ScaleX * wnd.ActualWidth;
            var scaledHeight = transform.ScaleY * wnd.ActualHeight;

            if (transform.ScaleX < minScale)
                transform.ScaleX = minScale;

            if (transform.ScaleY < minScale)
                transform.ScaleY = minScale;

            if (maxScale != double.MaxValue)
            {
                if (transform.ScaleX > maxScale)
                    transform.ScaleX = maxScale;

                if (transform.ScaleY > maxScale)
                    transform.ScaleY = maxScale;
            }
            else
            {
                if (transform.TranslateY + scaledHeight >= _workspace.ActualHeight)
                {
                    transform.TranslateY = _workspace.ActualHeight - scaledHeight;
                    if (transform.TranslateY < 0)
                    {
                        var maxContainerScale = _workspace.ActualHeight / wnd.ActualHeight;
                        transform.ScaleX = maxContainerScale;
                        transform.ScaleY = maxContainerScale;
                    }
                }
                if (transform.TranslateX + scaledWidth >= _workspace.ActualWidth)
                {
                    transform.TranslateX = _workspace.ActualWidth - scaledWidth;
                    if (transform.TranslateX < 0)
                    {
                        var maxContainerScale = _workspace.ActualWidth / wnd.ActualWidth;
                        transform.ScaleX = maxContainerScale;
                        transform.ScaleY = maxContainerScale;
                    }
                }
            }
            compositeTransform.ScaleX = 1.0d / transform.ScaleX;
            compositeTransform.ScaleY = 1.0d / transform.ScaleY;

        }
        private void PerformTransation(Window wnd, ManipulationDeltaRoutedEventArgs e, CompositeTransform transform)
        {
            if (wnd.IsFullscreen)
                return;

            transform.TranslateX += e.Delta.Translation.X;
            transform.TranslateY += e.Delta.Translation.Y;
        }

        #endregion

        private void PART_Menu_MenuClosed(object sender, EventArgs e)
        {
            _workspace.WindowDataSource.Clear();
        }
    }
}
