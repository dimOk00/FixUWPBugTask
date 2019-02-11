using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GhostCore.MVVM.Messaging;

namespace Foundation.Core.Events.Windows
{
    public class WindowOperationEventData
    {
        public WindowOperation Operation { get; set; }
        public object WindowObject { get; set; }
        public object DataObject { get; set; }
        public BusEvent BusEvent { get; set; }
    }
}
