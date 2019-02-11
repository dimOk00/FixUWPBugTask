using GhostCore.MVVM;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Foundation.Core.Data;
using System.Collections.Generic;
using System.Collections;

namespace Foundation.Core.ViewModels
{
    public class SessionViewModel : ViewModelBase<Session>
    {
        private ApplicationSettingsViewModel _root;
        private ObservableCollection<ContentItemViewModel> _srcForTree;

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; OnPropertyChanged(nameof(Name)); }
        }

        public ApplicationSettingsViewModel Root
        {
            get { return _root; }
            set
            {
                _root = value;
                OnPropertyChanged(nameof(Root));
                TreeSource = new ObservableCollection<ContentItemViewModel>() { _root };
            }
        }

        [JsonIgnore]
        public ObservableCollection<ContentItemViewModel> TreeSource
        {
            get { return _srcForTree; }
            set { _srcForTree = value; OnPropertyChanged(nameof(TreeSource)); }
        }

        public SessionViewModel(Session model)
            : base(model)
        {
            Root = new ApplicationSettingsViewModel(model.ApplicationSettings);
        }

        public List<ContentItemViewModel> Flatten()
        {
            var rv = new List<ContentItemViewModel>();

            Visit(rv, Root);

            return rv;
        }

        private void Visit(List<ContentItemViewModel> target, ContentItemViewModel item)
        {
            if (item.Children == null || item.Children.Count == 0)
            {
                target.Add(item);
                return;
            }

            foreach (var contentItemViewModel in item.Children)
            {
                Visit(target, contentItemViewModel);
            }

            if (item is HostedLayoutViewModel)
            {
                target.Add(item);
            }
        }
    }


}
