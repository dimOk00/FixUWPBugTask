using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Foundation.Core.Data.Abstract
{
    public interface IMultimediaItem
    {
        Task<Size> GetSize();
    }
}
