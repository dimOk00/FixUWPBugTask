using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Foundation.Controls.Imaging
{
    public class ThumbnailImage : UnrestrictedImage
    {
        public double ReducedScaleFactor
        {
            get { return (double)GetValue(ReducedScaleFactorProperty); }
            set { SetValue(ReducedScaleFactorProperty, value); }
        }

        public static readonly DependencyProperty ReducedScaleFactorProperty =
            DependencyProperty.Register("ReducedScaleFactor", typeof(double), typeof(ThumbnailImage), new PropertyMetadata(1));

        protected override async Task SetImageSource(Uri uri)
        {
            if (PART_Image == null || uri == null)
                return;

            var local = uri.LocalPath;
            var file = await StorageFile.GetFileFromPathAsync(local);

            var imgInfo = await file.Properties.GetImagePropertiesAsync();

            using (var stream = await file.OpenReadAsync())
            {
                var bitmapImage = new BitmapImage();
                PART_Image.Source = bitmapImage;
                await bitmapImage.SetSourceAsync(stream);
            }

            //if (PART_Image == null || uri == null)
            //    return;

            //var local = uri.LocalPath;
            //var file = await StorageFile.GetFileFromPathAsync(local);

            //using (var thumb = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView))
            //{
            //    var bitmapImage = new BitmapImage();
            //    PART_Image.Source = bitmapImage;
            //    await bitmapImage.SetSourceAsync(thumb);
            //}
        }
    }
}
