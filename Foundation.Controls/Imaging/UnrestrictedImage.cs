using Foundation.Controls.Abstract;
using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Foundation.Controls.Imaging
{
    [TemplatePart(Name = "PART_Image", Type = typeof(Image))]
    public class UnrestrictedImage : Control, IMultimediaControl
    {
        public event ExceptionRoutedEventHandler ImageFailed;
        public event RoutedEventHandler ImageOpened;

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        public bool RequiresAuthorization
        {
            get { return (bool)GetValue(RequiresAuthorizationProperty); }
            set { SetValue(RequiresAuthorizationProperty, value); }
        }

        public string AuthorizationToken
        {
            get { return (string)GetValue(AuthorizationTokenProperty); }
            set { SetValue(AuthorizationTokenProperty, value); }
        }

        public Image PART_Image { get; private set; }


        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(UnrestrictedImage), new PropertyMetadata(null, OnUriSourceChanged));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(UnrestrictedImage), new PropertyMetadata(Stretch.Uniform));

        public static readonly DependencyProperty AuthorizationTokenProperty =
            DependencyProperty.Register("AuthorizationToken", typeof(string), typeof(UnrestrictedImage), new PropertyMetadata(null));

        public static readonly DependencyProperty RequiresAuthorizationProperty =
            DependencyProperty.Register("RequiresAuthorization", typeof(bool), typeof(UnrestrictedImage), new PropertyMetadata(false));


        public UnrestrictedImage()
        {
            DefaultStyleKey = typeof(UnrestrictedImage);
        }

        protected override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Image = GetTemplateChild(nameof(PART_Image)) as Image;
            PART_Image.ImageOpened += (object sender, RoutedEventArgs e) => ImageOpened?.Invoke(sender, e);
            PART_Image.ImageFailed += (object sender, ExceptionRoutedEventArgs e) => ImageFailed?.Invoke(sender, e);

            await SetImageSource(UriSource);
        }


        private static async void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as UnrestrictedImage;

            var uri = e.NewValue as Uri;
            await that.SetImageSource(uri);
        }

        protected virtual async Task SetImageSource(Uri uri)
        {
            if (PART_Image == null)
                return;

            if (uri == null)
            {
                PART_Image.Source = null;
                return;
            }

            if (uri.Scheme == "https" || uri.Scheme == "http")
            {
                if (RequiresAuthorization)
                    await SetAuthorizedImageSource(uri);
                else
                    PART_Image.Source = new BitmapImage(uri);
            }

            if (uri.Scheme == "file")
            {
                var local = uri.LocalPath;
                var file = await StorageFile.GetFileFromPathAsync(local);
                using (var stream = await file.OpenReadAsync())
                {
                    var bitmapImage = new BitmapImage();
                    PART_Image.Source = bitmapImage;
                    await bitmapImage.SetSourceAsync(stream);
                }
                return;
            }
        }

        private Task SetAuthorizedImageSource(Uri uri)
        {
            throw new NotImplementedException(); //for stefan
        }

        public async Task<Size> GetExpectedSize()
        {
            double aw = 0, ah = 0;
            aw = PART_Image.ActualWidth;
            ah = PART_Image.ActualHeight;

            if (aw == 0 || ah == 0)
            {
                var dc = PART_Image.DataContext as ImageContentItemViewModel;
                var sf = await StorageFile.GetFileFromPathAsync(dc.Path.LocalPath);
                var imgProps = await sf.Properties.GetImagePropertiesAsync();
                var aspectRatio = (double)imgProps.Width / imgProps.Height;
                ah = ActualHeight;
                aw = ActualHeight * aspectRatio;
            }

            return new Size(aw, ah);
        }

        public void Activate()
        {
            // Do nothing
        }

        public void Deactivate()
        {
            // Do nothing
        }
    }
}
