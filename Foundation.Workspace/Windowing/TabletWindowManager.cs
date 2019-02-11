using Foundation.Core.Data;
using Foundation.Core.Events.Windows;
using Foundation.Core.ViewModels;
using Foundation.Shared.Net.Events;
using GhostCore.MVVM.Messaging;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Window = Foundation.Workspace.Windowing.WorkspaceWindow;

namespace Foundation.Workspace.Windowing
{
    public class TabletWindowManager : WindowManagerBase
    {
        public TabletWindowManager(Workspace workspace)
            : base(workspace)
        {
            _wndOpHandlers = new Dictionary<WindowOperation, Action<WindowOperationEventData>>
            {
                { WindowOperation.Opened, WndOpHandler_Opened },
                { WindowOperation.Close, WndOpHandler_Close }
            };

            _workspace.Loaded += Workspace_Loaded;
        }


        private void Workspace_Loaded(object sender, RoutedEventArgs e)
        {
            _workspace.PART_Menu.MenuClosed += PART_Menu_MenuClosed;
        }

        protected override void SelectedMenuItemBus_EventBroadcasted(BusEvent e)
        {
            if (e.DataObject is ContentItemViewModel cvi && cvi.Type != ContentItemType.MenuItem)
            {
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

                if (cvi.PublishRemoteEvents)
                    _remoteBus.Publish(new RemoteEvent(RemoteEventType.MenuItemSelected, cvi.Model), this);

                if (e.OriginalSource is Menu.Menu)
                {
                    _workspace.WindowDataSource.Clear();
                }

                _workspace.WindowDataSource.Add(cvi);

                var cont = (ListViewItem)_workspace.PART_ItemsHost.ContainerFromItem(cvi);

                if (cont == null)
                    return;

                cont.Width = _workspace.PART_ItemsHost.ActualWidth;
                cont.Height = _workspace.PART_ItemsHost.ActualHeight;
            }
        }

        #region Window Operation Callbacks
        private void WndOpHandler_Opened(WindowOperationEventData obj)
        {
            var vm = obj.DataObject as ContentItemViewModel;

            if (obj.WindowObject is Window wnd)
            {
                wnd.SetPosition(0, 0);

                // HACK : this should be temporary since i don't know if it's ok to have multiple windows open under the same VM
                if (!_vmWindowMapping.ContainsKey(vm))
                    _vmWindowMapping.Add(vm, wnd);

                wnd.PrepareFullscreen();

                BringItemToFront(vm);

            }
        }
        private void WndOpHandler_Close(WindowOperationEventData data)
        {
            if (data.DataObject is ContentItemViewModel cvi && cvi.Type != ContentItemType.MenuItem)
            {
                _workspace.WindowDataSource.Remove(cvi);
                _vmWindowMapping.Remove(cvi);

                if (cvi.PublishRemoteEvents)
                    _remoteBus.Publish(new RemoteEvent(RemoteEventType.WindowClosed, cvi.Model), this);
            }
        }

        
        #endregion

        private void PART_Menu_MenuClosed(object sender, EventArgs e)
        {
            _workspace.WindowDataSource.Clear();
            _remoteBus.Publish(new RemoteEvent(RemoteEventType.MenuClosed, null), this);
        }
    }
}
