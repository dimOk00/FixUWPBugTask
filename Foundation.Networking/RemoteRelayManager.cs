using Foundation.Shared.Net;
using Foundation.Shared.Net.Events;
using GhostCore.MVVM;
using GhostCore.MVVM.Messaging;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Networking
{
    public sealed class RemoteRelayManager
    {
        #region Singleton

        private static volatile RemoteRelayManager _instance;
        private static object _syncRoot = new object();


        public static RemoteRelayManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new RemoteRelayManager();
                    }
                }

                return _instance;
            }
        }
        private RemoteRelayManager()
        {
        }

        #endregion

        #region Fields

        private IEventSerializer _eventSerializer;
        private IRelayHubCallbackHandler _handler;
        private EventBus _remoteEventBus;
        private ClientInfo _localClientInfo;

        #endregion

        #region Properties


        public HubConnection Connection { get; set; }
        public bool IsConnected { get; set; }
        public RemoteRelayConfiguration Configuration { get; set; }
        public ClientInfo SelectedTarget { get; set; }

        #endregion

        #region API

        public async Task Initialize(RemoteRelayConfiguration configuration)
        {
            try
            {
                Configuration = configuration;
                _handler = configuration.Handler;
                _eventSerializer = configuration.EventSerializer;

                Connection = new HubConnectionBuilder()
                    .WithUrl(Configuration.ServerURL, HttpTransportType.WebSockets)
                    .AddMessagePackProtocol()
                    .Build();

                Connection.Closed += Connection_Closed;

                await Connection.StartAsync();
                IsConnected = true;

                _localClientInfo = new ClientInfo() { ClientIdentifier = Guid.NewGuid().ToString(), HostType = HostType.TouchscreenWall };

                await Connection.SendAsync(nameof(IRelayHub.RegisterClient), _localClientInfo);
                await Connection.SendAsync(nameof(IRelayHub.GetOnlineClients), _localClientInfo);

                RegisterRemoteEvents();
                PrepareEventHubs();
            }
            catch (Exception ex)
            {
                Debug.Log($"[RMEOTE] Did not connect to server. Reason: {ex.Message}");
            }
        }

        private void PrepareEventHubs()
        {
            _remoteEventBus = EventBusManager.Instance.GetRemoteEventBus();
            _remoteEventBus.EventBroadcasted += RemoteEventBus_EventBroadcasted;
        }

        private void RegisterRemoteEvents()
        {
            Connection.On<List<ClientInfo>>(nameof(IRelayHubCallbackHandler.OnGetOnlineClients), _handler.OnGetOnlineClients);
            Connection.On<byte[]>(nameof(IRelayHubCallbackHandler.OnRelayMessage), _handler.OnRelayMessage);
            Connection.On<string>(nameof(IRelayHubCallbackHandler.OnRelayMessageFail), _handler.OnRelayMessageFail);
        }

        public async Task Close()
        {
            await Connection.StopAsync();
            Connection.Closed -= Connection_Closed;

            await Connection.DisposeAsync();
            Connection = null;
        }

        #endregion

        private void RemoteEventBus_EventBroadcasted(BusEvent e)
        {
            if (SelectedTarget == null)
            {
                if (_handler.OnlineClients.Count == 0)
                    return;

                SelectedTarget = _handler.OnlineClients[0]; //TODO: temp
            }

            var eData = e.DataAs<RemoteEvent>();
            var seri = _eventSerializer.Serialize(eData);
            Connection.SendAsync(nameof(IRelayHub.RelayMessage), SelectedTarget, seri);
        }

        private Task Connection_Closed(Exception ex)
        {
            if (ex != null)
                Debug.Log(ex);

            IsConnected = false;
            return Task.CompletedTask;
        }
    }
}
