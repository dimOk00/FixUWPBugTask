using GhostCore.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GhostCore.UWP.MVVM
{
    public class PageViewModel<T> : ViewModelBase where T : Page
    {
        private T _page;

        public T Page
        {
            get { return _page; }
            set { _page = value; OnPropertyChanged(nameof(Page)); }
        }

        public PageViewModel()
        {

        }

        public PageViewModel(T page)
        {
            _page = page;
        }
    }
}
