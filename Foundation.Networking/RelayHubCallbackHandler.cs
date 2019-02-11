using Foundation.Shared.Net;
using Foundation.Shared.Net.Events;
using GhostCore;
using GhostCore.MVVM;
using GhostCore.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Foundation.Networking
{
    public class RelayHubCallbackHandler : IRelayHubCallbackHandler
    {
        private IEventSerializer _serializer;
        private Dictionary<RemoteEventType, Action<RemoteEvent>> _callbacks;
        private EventBus _selectedMenuItemBus;
        private EventBus _remoteWindowOperationsBus;
        private EventBus _remoteMultimediaOpsBus;

        public ObservableCollection<ClientInfo> OnlineClients { get; set; }


        public RelayHubCallbackHandler(IEventSerializer serializer)
        {
            _serializer = serializer;
            _callbacks = new Dictionary<RemoteEventType, Action<RemoteEvent>>()
            {
                { RemoteEventType.MenuItemSelected, OnMenuItemSelected },
                { RemoteEventType.MenuClosed, OnMenuClosed },
                { RemoteEventType.WindowClosed, OnWindowClosed },
                { RemoteEventType.MultimediaControlSelectionChanged, OnMultimediaControlSelectionChanged }
            };

            _selectedMenuItemBus = EventBusManager.Instance.GetOrCreateBus("MenuSelectedItemBus");
            _remoteWindowOperationsBus = EventBusManager.Instance.GetOrCreateBus("RemoteWindowOperationBus");
            _remoteMultimediaOpsBus = EventBusManager.Instance.GetOrCreateBus("RemoteMultimediaOpsBus");

            OnlineClients = new ObservableCollection<ClientInfo>();
        }

        public void OnGetOnlineClients(List<ClientInfo> clients)
        {
            foreach (var x in clients)
            {
                if (OnlineClients.Contains(x))
                    continue;

                OnlineClients.Add(x);
            }
        }

        public void OnRelayMessage(byte[] data)
        {
            var remoteEvt = _serializer.Deserialize(data);
            if (_callbacks.ContainsKey(remoteEvt.Type))
                _callbacks[remoteEvt.Type](remoteEvt);
            else
                Debug.Log($"No follow up for relayed message of type {remoteEvt.Type}");
        }

        public void OnRelayMessageFail(string info)
        {
            Debug.Log($"Failed to relay message: {info}");
        }

        private void OnMenuItemSelected(RemoteEvent obj)
        {
            var vm = ViewModelMapper.GetViewModel(obj.Payload);
            _selectedMenuItemBus.Publish("remote", vm, obj);
        }
        private void OnMenuClosed(RemoteEvent obj)
        {
            _selectedMenuItemBus.Publish("remote", null, obj);
        }
        private void OnWindowClosed(RemoteEvent obj)
        {
            var vm = ViewModelMapper.GetViewModel(obj.Payload);
            _remoteWindowOperationsBus.Publish(new RemoteWindowOperationEvent() { RemoteEvent = obj, ViewModel = vm }, this);
        }
        private void OnMultimediaControlSelectionChanged(RemoteEvent obj)
        {
            _remoteMultimediaOpsBus.Publish(obj, this);
        }

    }

    public class RemoteWindowOperationEvent
    {
        public RemoteEvent RemoteEvent { get; internal set; }
        public INotifyPropertyChanged ViewModel { get; internal set; }
    }
}
