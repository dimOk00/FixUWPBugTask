using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Shared.Net.Events
{
    public class RemoteEvent
    {
        public RemoteEventType Type { get; set; }
        public object Payload { get; set; }
        public RemoteEvent(RemoteEventType type, object payload)
        {
            Type = type;
            Payload = payload;
        }

    }

    public enum RemoteEventType : byte
    {
        MenuClosed,
        MoveMenuTo,
        MenuItemSelected,
        WindowClosed,
        WindowLocked,
        WindowRestored,
        WindowUnlocked,
        MultimediaControlSelectionChanged
    }

    public class MoveToPayload
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool Animate { get; set; }
    }

    public class SelectionChangedPayload
    {
        public int SelectedIndex { get; set; }
        public object Context { get; set; }
    }
}
