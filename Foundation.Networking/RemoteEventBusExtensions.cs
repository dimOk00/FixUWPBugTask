using GhostCore.MVVM.Messaging;

namespace Foundation.Networking
{
    public static class RemoteEventBusExtensions
    {
        private static string _remoteEventBusName = "RemoteEventBus";

        public static EventBus GetRemoteEventBus(this EventBusManager man)
        {
            return man.GetOrCreateBus(_remoteEventBusName);
        }
    }
}
