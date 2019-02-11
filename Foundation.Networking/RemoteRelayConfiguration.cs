using Foundation.Shared.Net;

namespace Foundation.Networking
{
    public class RemoteRelayConfiguration
    {
        public string ServerURL { get; set; }
        public IRelayHubCallbackHandler Handler { get; set; }
        public IEventSerializer EventSerializer { get; set; }
    }
}
