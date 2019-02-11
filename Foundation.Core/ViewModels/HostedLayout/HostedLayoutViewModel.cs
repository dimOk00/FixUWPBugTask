using Foundation.Core.Data;
using GhostCore.Collections;
using GhostCore.MVVM;
using GhostCore.UWP.AutoFormGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core.ViewModels
{
    public class HostedLayoutViewModel : ContentItemViewModel
    {
        private ViewModelCollection<HostedLayoutPageViewModel, HostedLayoutPage> _pages;
        private HostedLayoutPageViewModel _selectedPage;

        public ViewModelCollection<HostedLayoutPageViewModel, HostedLayoutPage> Pages
        {
            get { return _pages; }
            set { _pages = value; OnPropertyChanged(nameof(Pages)); }
        }

        [HiddenFormItem]
        public HostedLayoutPageViewModel SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                OnPropertyChanged(nameof(SelectedPage));
                RemovePage.ProcExecuteChanged();
            }
        }

        [HiddenFormItem]
        public bool SkipCustomEditorCheck { get; set; }

        [HiddenFormItem]
        public RelayCommand AddPage { get; set; }

        [HiddenFormItem]
        public RelayCommand RemovePage { get; set; }

        public HostedLayoutViewModel(ContentItem model)
          : base(model)
        {
            AddPage = new RelayCommand(OnAddPage);
            RemovePage = new RelayCommand(OnRemovePage, CanExecuteRemovePage);


            var hostedLayout = ModelAs<HostedLayout>();
            var lst = new List<HostedLayoutPageViewModel>();

            foreach (var x in hostedLayout.Pages)
            {
                var vm = (HostedLayoutPageViewModel)ViewModelMapper.GetViewModel(x);
                if (vm == null)
                    vm = new HostedLayoutPageViewModel(x);

                vm.Parent = this;
                lst.Add(vm);
            }

            foreach (var x in lst)
            {
                x.RelinkBrokenPageReferences(lst);
            }

            _pages = new ViewModelCollection<HostedLayoutPageViewModel, HostedLayoutPage>(hostedLayout.Pages, lst);
        }

        private bool CanExecuteRemovePage(object arg)
        {
            return SelectedPage != null;
        }

        private void OnRemovePage(object obj)
        {
            _pages.Remove(_selectedPage);
        }

        private void OnAddPage(object obj)
        {
            var page = new HostedLayoutPageViewModel(new HostedLayoutPage(ModelAs<HostedLayout>())
            {
                Name = $"New Page {_pages.Count + 1}",
                CanvasWidth = 3840,
                CanvasHeight = 2160
            });
            _pages.Add(page);
            SelectedPage = page;
        }
    }
}
