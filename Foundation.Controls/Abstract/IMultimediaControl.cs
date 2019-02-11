using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Foundation.Controls.Abstract
{
    public interface IMultimediaControl
    {
        Task<Size> GetExpectedSize();
        void Activate();
        void Deactivate();
    }
}
