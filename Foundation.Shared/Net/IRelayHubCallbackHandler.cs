using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Shared.Net
{

    public interface IRelayHubCallbackHandler
    {
        ObservableCollection<ClientInfo> OnlineClients { get; set; }

        void OnGetOnlineClients(List<ClientInfo> clients);
        void OnRelayMessage(byte[] data);
        void OnRelayMessageFail(string info);
    }

}
