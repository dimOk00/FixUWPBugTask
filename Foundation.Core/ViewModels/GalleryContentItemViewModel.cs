using Foundation.Core.Data;
using GhostCore.UWP.AutoFormGeneration;
using System;

namespace Foundation.Core.ViewModels
{
    public class GalleryContentItemViewModel : ContentItemViewModel
    {
        public bool AutoInferFromPath
        {
            get { return ModelAs<GalleryContentItem>().AutoInferFromPath; }
            set
            {
                ModelAs<GalleryContentItem>().AutoInferFromPath = value;
                OnPropertyChanged(nameof(AutoInferFromPath));
            }
        }

        [HiddenFormItem]
        public bool NotAutoInferFromPath => !AutoInferFromPath;

        [BrowseFormItem(Label = "Folder Path", BrowseForFolder = true)]
        [VisibleIf(BooleanSourceProperty = nameof(AutoInferFromPath))]
        public Uri Path
        {
            get { return ModelAs<GalleryContentItem>().Path; }
            set { ModelAs<GalleryContentItem>().Path = value; OnPropertyChanged(nameof(Path)); }
        }

        public GalleryContentItemViewModel(ContentItem model)
           : base(model)
        {
        }
    }

}
