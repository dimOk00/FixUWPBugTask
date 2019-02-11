using Foundation.Shared.Net.Events;
using System;

namespace Foundation.Networking
{
    public interface IEventSerializer
    {
        byte[] Serialize(RemoteEvent obj);
        RemoteEvent Deserialize(byte[] data);
    }
}
