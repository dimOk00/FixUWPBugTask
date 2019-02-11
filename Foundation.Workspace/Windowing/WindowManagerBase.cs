using Foundation.Core.Events.Windows;
using Foundation.Core.ViewModels;
using GhostCore;
using GhostCore.MVVM.Messaging;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Foundation.Networking;
using Window = Foundation.Workspace.Windowing.WorkspaceWindow;

namespace Foundation.Workspace.Windowing
{
    public abstract class WindowManagerBase
    {
        #region Fields

        protected int _currentZIndex;

        protected Workspace _workspace;
        protected RootConfigurationViewModel _rootConfigVm;
        protected CascadeManager _cascadeManager;

        protected Dictionary<WindowOperation, Action<WindowOperationEventData>> _wndOpHandlers;
        protected Dictionary<ContentItemViewModel, Window> _vmWindowMapping;

        protected EventBus _selectedMenuItemBus;
        protected EventBus _windowOperationsBus;
        protected EventBus _remoteWindowOperationsBus;
        protected EventBus _remoteBus;

        #endregion


        public WindowManagerBase(Workspace workspace)
        {
            _workspace = workspace;
            _rootConfigVm = ServiceLocator.Instance.Resolve<RootConfigurationViewModel>();

            _vmWindowMapping = new Dictionary<ContentItemViewModel, Window>();

            _selectedMenuItemBus = EventBusManager.Instance.GetOrCreateBus(nameof(ContentItemViewModel.MenuSelectedItemBus));
            _windowOperationsBus = EventBusManager.Instance.GetOrCreateBus("WindowOperationBus");
            _remoteWindowOperationsBus = EventBusManager.Instance.GetOrCreateBus("RemoteWindowOperationBus");
            _remoteBus = EventBusManager.Instance.GetRemoteEventBus();


            _windowOperationsBus.EventBroadcasted += WindowOperationsBus_EventBroadcasted;
            _selectedMenuItemBus.EventBroadcasted += SelectedMenuItemBus_EventBroadcasted;
            _remoteWindowOperationsBus.EventBroadcasted += RemoteWindowOperationsBus_EventBroadcasted;

            _cascadeManager = new CascadeManager(_workspace)
            {
                HostType = _rootConfigVm.HostType
            };
        }

        protected virtual void WindowOperationsBus_EventBroadcasted(BusEvent e)
        {
            var eventData = e.DataAs<WindowOperationEventData>();
            eventData.BusEvent = e;

            if (!_wndOpHandlers.ContainsKey(eventData.Operation))
            {
                Debug.Log($"[WARNING] No mapped handler exists for operation: {eventData.Operation}");
                return;
            }

            _wndOpHandlers[eventData.Operation](eventData);
        }
        protected abstract void SelectedMenuItemBus_EventBroadcasted(BusEvent e);

        protected virtual void RemoteWindowOperationsBus_EventBroadcasted(BusEvent e)
        {
            
        }


        protected void BringItemToFront(ContentItemViewModel vm)
        {
            var cont = (ListViewItem)_workspace.PART_ItemsHost.ContainerFromItem(vm);

            if (cont == null)
                return;

            Canvas.SetZIndex(cont, ++_currentZIndex);
        }
    }
}
