using Foundation.Core.Data;
using Foundation.Core.Data.Abstract;
using GhostCore.UWP.AutoFormGeneration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Foundation.Core.ViewModels
{
    public class ImageContentItemViewModel : ContentItemViewModel, IMultimediaItem
    {
        [BrowseFormItem(Label = "Image Path")]
        public Uri Path
        {
            get { return ModelAs<ImageContentItem>().Path; }
            set { ModelAs<ImageContentItem>().Path = value; OnPropertyChanged(nameof(Path)); }
        }
        public ImageContentItemViewModel(ContentItem model)
           : base(model)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is ImageContentItemViewModel model &&
                   EqualityComparer<Uri>.Default.Equals(Path, model.Path);
        }

        public override int GetHashCode()
        {
            return 467214278 + EqualityComparer<Uri>.Default.GetHashCode(Path);
        }

        public async Task<Size> GetSize()
        {
            var sf = await StorageFile.GetFileFromPathAsync(Path.LocalPath);
            var imgProps = await sf.Properties.GetImagePropertiesAsync();
            return new Size(imgProps.Width, imgProps.Height);
        }
    }

}
