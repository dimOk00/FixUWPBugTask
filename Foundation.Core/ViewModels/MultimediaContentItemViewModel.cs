using Foundation.Core.Data;
using GhostCore.MVVM.Messaging;
using GhostCore.UWP.AutoFormGeneration;
using System;

namespace Foundation.Core.ViewModels
{
    public class MultimediaContentItemViewModel : ContentItemViewModel
    {
        private object _selectedMultimediaItem;

        public bool AutoInferFromPath
        {
            get { return ModelAs<MultimediaContentItem>().AutoInferFromPath; }
            set
            {
                ModelAs<MultimediaContentItem>().AutoInferFromPath = value;
                OnPropertyChanged(nameof(AutoInferFromPath));
            }
        }

        [BrowseFormItem(Label = "Folder Path", BrowseForFolder = true)]
        [VisibleIf(BooleanSourceProperty = nameof(AutoInferFromPath))]
        public Uri Path
        {
            get { return ModelAs<MultimediaContentItem>().Path; }
            set { ModelAs<MultimediaContentItem>().Path = value; OnPropertyChanged(nameof(Path)); }
        }

        public bool IsSlideshow
        {
            get { return ModelAs<MultimediaContentItem>().IsSlideshow; }
            set { ModelAs<MultimediaContentItem>().IsSlideshow = value; OnPropertyChanged(nameof(IsSlideshow)); }
        }

        public TimeSpan SlideInterval
        {
            get { return ModelAs<MultimediaContentItem>().SlideInterval; }
            set { ModelAs<MultimediaContentItem>().SlideInterval = value; OnPropertyChanged(nameof(SlideInterval)); }
        }

        [HiddenFormItem]
        public bool NotAutoInferFromPath => !AutoInferFromPath;

        [HiddenFormItem]
        public object SelectedMultimediaItem
        {
            get { return _selectedMultimediaItem; }
            set
            {
                _selectedMultimediaItem = value;
                OnPropertyChanged(nameof(SelectedMultimediaItem));
                MenuSelectedItemBus.Publish(this, this);
            }
        }

        public MultimediaContentItemViewModel(ContentItem model)
           : base(model)
        {
            
        }

        private void SetSelectedMultimediaItem(object val)
        {
            _selectedMultimediaItem = val;
        }

        public override ContentItemViewModel Clone()
        {
            var val = new MultimediaContentItemViewModel(Model);
            val.SetSelectedMultimediaItem(SelectedMultimediaItem);
            return val;
        }
    }

}
