using Foundation.Shared.Net.Events;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;

namespace Foundation.Networking
{
    public class DefaultEventSerializer : IEventSerializer
    {
        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        public unsafe RemoteEvent Deserialize(byte[] data)
        {
            var payloadLen = data.Length - 1;
            var type = (RemoteEventType)data[0];
            var slice = data.AsMemory().Slice(1);
            var pinnedSlice = slice.Pin();
            var seri = Encoding.Default.GetString((byte*)pinnedSlice.Pointer, payloadLen);
            pinnedSlice.Dispose();

            var payload = JsonConvert.DeserializeObject(seri, _serializerSettings);

            return new RemoteEvent(type, payload);
        }

        public byte[] Serialize(RemoteEvent obj)
        {
            var payload = obj.Payload;

            var seri = JsonConvert.SerializeObject(payload, _serializerSettings);

            var payloadBa = Encoding.Default.GetBytes(seri);
            var rv = new byte[payloadBa.Length + 1];

            rv[0] = (byte)obj.Type;
            Array.Copy(payloadBa, 0, rv, 1, payloadBa.Length);

            return rv;
        }
    }
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class EventSerializerIgnoreAttribute : Attribute
    {
        
    }
}
