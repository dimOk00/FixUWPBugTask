using System.Threading.Tasks;
using Foundation.Shared.Net;

namespace Foundation.Shared.Net
{
    public interface IRelayHub
    {
        Task GetOnlineClients(ClientInfo msg);
        Task RegisterClient(ClientInfo message);
        Task RelayMessage(ClientInfo target, byte[] data);
    }
}
