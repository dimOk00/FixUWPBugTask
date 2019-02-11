using Foundation.Core.Data;
using GhostCore.Collections;
using GhostCore.MVVM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foundation.Core.ViewModels
{
    public class HostedLayoutPageViewModel : ViewModelBase<HostedLayoutPage>
    {
        private ViewModelCollection<HostedLayoutItemViewModel, HostedLayoutItem> _items;
        private HostedLayoutItemViewModel _selectedItem;

        public HostedLayoutItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; OnPropertyChanged(nameof(Name)); }
        }

        public Uri BackgroundPath
        {
            get { return Model.BackgroundPath; }
            set { Model.BackgroundPath = value; OnPropertyChanged(nameof(BackgroundPath)); }
        }

        public int BackgroundColor
        {
            get { return Model.BackgroundColor; }
            set { Model.BackgroundColor = value; OnPropertyChanged(nameof(BackgroundColor)); }
        }

        public double CanvasWidth
        {
            get { return Model.CanvasWidth; }
            set { Model.CanvasWidth = value; OnPropertyChanged(nameof(CanvasWidth)); }
        }

        public double CanvasHeight
        {
            get { return Model.CanvasHeight; }
            set { Model.CanvasHeight = value; OnPropertyChanged(nameof(CanvasHeight)); }
        }

        public ViewModelCollection<HostedLayoutItemViewModel, HostedLayoutItem> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(nameof(Items)); }
        }

        public ObservableCollection<ContentAreaViewModel> ContentAreas { get; set; }

        public RelayCommand RemoveItem { get; set; }

        public HostedLayoutPageViewModel(HostedLayoutPage model)
            : base(model)
        {
            RemoveItem = new RelayCommand(OnRemoveItem, CanExecuteRemoveItem);
            var lst = new List<HostedLayoutItemViewModel>();

            foreach (var x in model.Items)
            {
                HostedLayoutItemViewModel vm = null;

                vm = (ContentAreaViewModel)ViewModelMapper.GetViewModel(x);
                if (vm == null)
                {
                    if (x is ContentArea)
                    {
                        vm = new ContentAreaViewModel(x);
                    }
                    else
                    if (x is ToggleItem)
                    {
                        vm = new ToggleItemViewModel(x);
                    }
                    else
                    if (x is ButtonItem)
                    {
                        vm = new ButtonItemViewModel(x);
                    }
                    else
                    if (x is InteractableItem)
                    {
                        vm = new InteractableItemViewModel(x);
                    }
                }

                if (vm == null)
                    continue;

                vm.Parent = this;

                lst.Add(vm);
            }
            _items = new ViewModelCollection<HostedLayoutItemViewModel, HostedLayoutItem>(model.Items, lst);
            _items.CollectionChanged += items_CollectionChanged;

            ContentAreas = new ObservableCollection<ContentAreaViewModel>(_items.Where(x => x is ContentAreaViewModel).Cast<ContentAreaViewModel>().ToList());
        }

        public void RelinkBrokenPageReferences(IEnumerable<HostedLayoutPageViewModel> pages)
        {
            foreach (var x in _items)
            {
                if (x is InteractableItemViewModel ivm)
                {
                    var interactableItem = ivm.ModelAs<InteractableItem>();
                    if (ivm.PageTarget == null && interactableItem.PageTargetName != null)
                    {
                        var page = pages.FirstOrDefault(q => q.Name == interactableItem.PageTargetName);
                        ivm.PageTarget = page;
                    }
                }
            }
        }

        private void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var x in e.NewItems)
                    {
                        if (x is ContentAreaViewModel ca)
                        {
                            ContentAreas.Add(ca);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var x in e.OldItems)
                    {
                        if (x is ContentAreaViewModel ca)
                        {
                            ContentAreas.Remove(ca);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void OnRemoveItem(object obj)
        {
            Items.Remove(SelectedItem);
        }

        private bool CanExecuteRemoveItem(object arg) => SelectedItem != null;
    }
}
