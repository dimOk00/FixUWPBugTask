using Foundation.Core.Data;
using GhostCore;
using GhostCore.Collections;
using GhostCore.MVVM;
using GhostCore.MVVM.Dynamic;
using GhostCore.MVVM.Messaging;
using GhostCore.UWP.AutoFormGeneration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Foundation.Core.ViewModels
{
    public class ContentItemViewModel : ViewModelBase<ContentItem>
    {
        #region Fields

        private static IViewModelFactory<ContentItemViewModel, ContentItem> _factory;

        private ViewModelCollection<ContentItemViewModel, ContentItem> _children;
        private object _selItem;
        private WindowSettingsViewModel _windowSettings;
        private double _opacity = 1;

        #endregion

        #region Properties

        [JsonIgnore]
        public EventBus MenuSelectedItemBus { get; set; }

        //[HiddenFormItem]
        public int Id
        {
            get { return Model.Id; }
            set { Model.Id = value; OnPropertyChanged(nameof(Id)); }
        }

        [HiddenFormItem]
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; OnPropertyChanged(nameof(Name)); }
        }

        [FormItem(Label = "Label")]
        public string DisplayLabel
        {
            get { return Model.DisplayLabel; }
            set { Model.DisplayLabel = value; OnPropertyChanged(nameof(DisplayLabel)); }
        }

        [FormItem(Label = "Is Hidden")]
        public bool IsHidden
        {
            get { return Model.IsHidden; }
            set { Model.IsHidden = value; OnPropertyChanged(nameof(IsHidden)); }
        }

        [HiddenFormItem]
        public ContentItemType Type
        {
            get { return Model.Type; }
            set { Model.Type = value; OnPropertyChanged(nameof(Type)); }
        }

        public bool IsSingleton
        {
            get { return Model.IsSingleton; }
            set { Model.IsSingleton = value; OnPropertyChanged(nameof(IsSingleton)); }
        }

        public object SelectedItem
        {
            get { return _selItem; }
            set
            {
                _selItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                MenuSelectedItemBus.Publish(_selItem, this);
            }
        }

        public HostType DisplayTarget
        {
            get { return Model.DisplayTarget; }
            set { Model.DisplayTarget = value; OnPropertyChanged(nameof(DisplayTarget)); }
        }

        [ExpandClassAsGroup(GroupLabel = "Window Settings")]
        public WindowSettingsViewModel WindowSettings
        {
            get { return _windowSettings; }
            set { _windowSettings = value; OnPropertyChanged(nameof(WindowSettings)); }
        }

        public ViewModelCollection<ContentItemViewModel, ContentItem> Children
        {
            get { return _children; }
            set { _children = value; OnPropertyChanged(nameof(Children)); }
        }

        [HiddenFormItem]
        public bool PublishRemoteEvents { get; set; } = true;



        [HiddenFormItem]
        public double Opacity
        {
            get { return _opacity; }
            set { _opacity = value; OnPropertyChanged(nameof(Opacity)); }
        }

        [HiddenFormItem]
        public bool ShouldAnimateOpacity { get; set; }

        #endregion

        #region Init

        static ContentItemViewModel()
        {
            _factory = ServiceLocator.Instance.Resolve<IViewModelFactory<ContentItemViewModel, ContentItem>>();
        }

        public ContentItemViewModel()
            : this(new ContentItem())
        {
        }

        public ContentItemViewModel(ContentItem model)
            : base(model)
        {
            MenuSelectedItemBus = EventBusManager.Instance.GetOrCreateBus(nameof(MenuSelectedItemBus));
            WindowSettings = new WindowSettingsViewModel(Model.WindowSettings);

            var lst = new List<ContentItemViewModel>();
            foreach (var item in model.Children)
            {
                var vm = CreateItem(item);
                vm.Parent = this;

                if (item.WindowSettings.InheritProperties)
                {
                    vm.WindowSettings.InheritFrom(WindowSettings);
                }

                lst.Add(vm);
            }

            _children = new ViewModelCollection<ContentItemViewModel, ContentItem>(Model.Children, lst);
        }

        public virtual ContentItemViewModel Clone()
        {
            return new ContentItemViewModel(Model);
        }

        public static ContentItemViewModel CreateItem(ContentItem item)
        {
            return _factory.Create(item);
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }

    public class DefaultContentItemViewModelFactory : IViewModelFactory<ContentItemViewModel, ContentItem>
    {
        public virtual ContentItemViewModel Create(ContentItem item)
        {
            switch (item.Type)
            {
                case ContentItemType.MenuItem:
                    return new ContentItemViewModel(item);
                case ContentItemType.Image:
                    return new ImageContentItemViewModel(item);
                case ContentItemType.Video:
                    return new VideoContentItemViewModel(item);
                case ContentItemType.PDF:
                    return new PDFContentItemViewModel(item);
                case ContentItemType.MultimediaControl:
                    return new MultimediaContentItemViewModel(item);
                case ContentItemType.Gallery:
                    return new GalleryContentItemViewModel(item);
                case ContentItemType.HostedLayout:
                    return new HostedLayoutViewModel(item);
                case ContentItemType.WebView:
                    return new WebViewContentItemViewModel(item);
                case ContentItemType.ProjectSpecific:
                    return GetProjectSpecific(item);
                default:
                    return new ContentItemViewModel(item);
            }
        }

        public virtual ContentItemViewModel GetProjectSpecific(ContentItem item)
        {
            return null;
        }
    }
}
