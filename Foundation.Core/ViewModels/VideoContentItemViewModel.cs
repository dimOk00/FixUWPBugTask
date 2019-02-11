using Foundation.Core.Data;
using GhostCore.UWP.AutoFormGeneration;
using System;
using System.Collections.Generic;

namespace Foundation.Core.ViewModels
{
    public class VideoContentItemViewModel : ContentItemViewModel
    {
        [BrowseFormItem(Label = "Video Path")]
        public Uri Path
        {
            get { return ModelAs<VideoContentItem>().Path; }
            set { ModelAs<VideoContentItem>().Path = value; OnPropertyChanged(nameof(Path)); }
        }
        public VideoContentItemViewModel(ContentItem model)
           : base(model)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is VideoContentItemViewModel model &&
                   EqualityComparer<Uri>.Default.Equals(Path, model.Path);
        }

        public override int GetHashCode()
        {
            return 467214278 + EqualityComparer<Uri>.Default.GetHashCode(Path);
        }
    }

}
