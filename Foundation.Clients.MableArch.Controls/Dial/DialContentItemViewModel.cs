using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class DialContentItemViewModel : ContentItemViewModel
    {
        public Uri JsonPath
        {
            get { return ModelAs<DialContentItem>().JsonPath; }
            set { ModelAs<DialContentItem>().JsonPath = value; OnPropertyChanged(nameof(JsonPath)); }
        }

        public DialContentItemViewModel(DialContentItem model)
            : base(model)
        {
        }
    }
}
