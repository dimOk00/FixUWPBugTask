using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using GhostCore.MVVM.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core
{
    public class FoundationConfiguration
    {
        public IViewModelFactory<ContentItemViewModel, ContentItem> ContentItemViewModelFactory { get; set; }

        public FoundationConfiguration()
        {
            ContentItemViewModelFactory = new DefaultContentItemViewModelFactory();
        }
    }
}
